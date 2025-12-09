import { createReadStream } from "fs";
import { createInterface } from "readline";
import getFilePath from "../inputFiles/pathHelper.ts";

type JunctionBox = readonly [number, number, number];
interface Pair {
  readonly left: JunctionBox;
  readonly right: JunctionBox;
  readonly distance: number;
}
interface CircuitResult {
  readonly circuits: Set<JunctionBox>[];
  readonly lastPair?: Pair;
}

export default async function Puzzle8() {
  const getJunctionBoxes = async (): Promise<JunctionBox[]> => {
    const path = getFilePath("input-8");
    const fs = createReadStream(path);

    const rl = createInterface({
      input: fs,
      crlfDelay: Infinity,
    });

    const junctionBoxes: JunctionBox[] = [];
    for await (const line of rl) {
      const [x, y, z] = line.split(",", 3).map((el) => parseInt(el ?? "x"));
      if (isNaN(x) || isNaN(y) || isNaN(z)) {
        continue;
      }

      junctionBoxes.push([x, y, z]);
    }

    return junctionBoxes;
  };

  const getDistance = (left: JunctionBox, right: JunctionBox): number => {
    const [p1, p2, p3] = left;
    const [q1, q2, q3] = right;

    return Math.sqrt(Math.pow(p1 - q1, 2) + Math.pow(p2 - q2, 2) + Math.pow(p3 - q3, 2));
  };

  const connectCircuitsUntilSingle = (boxes: JunctionBox[], limit: number): CircuitResult => {
    const sortedPairs: Pair[] = [];
    const circuits: Set<JunctionBox>[] = boxes.map((box) => new Set([box]));

    for (let i = 0; i < boxes.length; i++) {
      for (let j = i + 1; j < boxes.length; j++) {
        const left = boxes[i];
        const right = boxes[j];

        const newPair: Pair = {
          left,
          right,
          distance: getDistance(left, right),
        };

        sortedPairs.push(newPair);
      }
    }
    sortedPairs.sort((left, right) => left.distance - right.distance);

    let lastPair: Pair | undefined = undefined;
    for (let i = 0; i < limit && circuits.length > 1; i++) {
      lastPair = sortedPairs.shift();
      if (lastPair === undefined) {
        break;
      }
      const { left, right } = lastPair;
      const leftCircuitIndex = circuits.findIndex((circuit) => circuit.has(left));
      const rightCircuitIndex = circuits.findIndex((circuit) => circuit.has(right));

      const leftCircuit = circuits[leftCircuitIndex];
      const rightCircuit = circuits[rightCircuitIndex];

      circuits[leftCircuitIndex] = leftCircuit.union(rightCircuit);

      if (leftCircuitIndex !== rightCircuitIndex) {
        circuits.splice(rightCircuitIndex, 1);
      }
    }

    return {
      circuits,
      lastPair,
    };
  };

  const getCircuitSizes = (boxes: JunctionBox[], limit: number): number => {
    const circuits = connectCircuitsUntilSingle(boxes, limit).circuits;

    circuits.sort((left, right) => right.size - left.size);
    return circuits.splice(0, 3).reduce((current, next) => current * next.size, 1);
  };

  const getLastPairDistance = (boxes: JunctionBox[]): number => {
    const lastPair = connectCircuitsUntilSingle(boxes, Number.MAX_SAFE_INTEGER).lastPair;
    return (lastPair?.left[0] ?? 0) * (lastPair?.right[0] ?? 0);
  };

  const boxes = await getJunctionBoxes();
  console.log(getCircuitSizes(boxes, 1000));
  console.log(getLastPairDistance(boxes));
}
