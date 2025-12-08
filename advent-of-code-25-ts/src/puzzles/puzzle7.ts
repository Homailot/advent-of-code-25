import { createReadStream } from "fs";
import { createInterface } from "readline";
import getFilePath from "../inputFiles/pathHelper.ts";

type Location = "." | "S" | "^";
type Position = [number, number];
type PositionWithCount = [number, number, number];
type Diagram = Location[][];

export default async function Puzzle7() {
  const getDiagram = async (): Promise<Diagram> => {
    const path = getFilePath("input-7");
    const fs = createReadStream(path);

    const rl = createInterface({
      input: fs,
      crlfDelay: Infinity,
    });

    const diagram: Diagram = [];
    for await (const line of rl) {
      const locations: Location[] = [];
      for (const location of line) {
        if (location === "." || location === "S" || location === "^") {
          locations.push(location);
        }
      }

      diagram.push(locations);
    }

    return diagram;
  };

  const getStartLocationIndex = (diagram: Diagram): number => {
    const locations = diagram.at(0);
    if (locations === undefined) {
      return -1;
    }

    for (const [index, location] of locations.entries()) {
      if (location === "S") {
        return index;
      }
    }

    return -1;
  };

  const addSplitIfEmpty = (beams: Position[], position: Position): boolean => {
    if (
      !beams.find(
        (beamPosition) =>
          beamPosition[0] === position[0] && beamPosition[1] === position[1],
      )
    ) {
      beams.push(position);
      return true;
    }
    return false;
  };

  const addSplitWithCount = (
    beams: PositionWithCount[],
    position: PositionWithCount,
  ) => {
    const index = beams.findIndex(
      (beamPosition) =>
        beamPosition[0] === position[0] && beamPosition[1] === position[1],
    );

    if (index === -1) {
      beams.push(position);
    } else {
      const [x, y, count] = beams[index];
      beams[index] = [x, y, count + position[2]];
    }
  };

  const countSplits = (diagram: Diagram): number => {
    const startIndex = getStartLocationIndex(diagram);
    if (startIndex === -1) {
      return 0;
    }

    let currentBeams: Position[] = [[startIndex, 1]];

    let count = 0;
    let [x, y] = currentBeams[0];
    while (currentBeams.length > 0) {
      currentBeams = currentBeams.splice(1);
      if (y >= diagram.length) {
        continue;
      }

      const location = diagram[y][x];
      if (location === "^") {
        const leftPosition: Position = [x - 1, y + 1];
        const rightPosition: Position = [x + 1, y + 1];
        count++;

        addSplitIfEmpty(currentBeams, leftPosition);
        addSplitIfEmpty(currentBeams, rightPosition);
      } else {
        addSplitIfEmpty(currentBeams, [x, y + 1]);
      }

      [x, y] = currentBeams[0];
    }

    return count;
  };

  const countTimelines = (diagram: Diagram): number => {
    const startIndex = getStartLocationIndex(diagram);
    if (startIndex === -1) {
      return 0;
    }

    let currentBeams: PositionWithCount[] = [[startIndex, 1, 1]];

    let [x, y, count] = currentBeams[0];
    let total = 0;
    while (currentBeams.length > 0) {
      currentBeams = currentBeams.splice(1);
      if (y >= diagram.length) {
        total += count;

        if (currentBeams.length > 0) {
          [x, y, count] = currentBeams[0];
        }
        continue;
      }

      const location = diagram[y][x];
      if (location === "^") {
        const leftPosition: PositionWithCount = [x - 1, y + 1, count];
        const rightPosition: PositionWithCount = [x + 1, y + 1, count];

        addSplitWithCount(currentBeams, leftPosition);
        addSplitWithCount(currentBeams, rightPosition);
      } else {
        addSplitWithCount(currentBeams, [x, y + 1, count]);
      }

      [x, y, count] = currentBeams[0];
    }

    return total;
  };

  const diagram = await getDiagram();
  console.log(countSplits(diagram));
  console.log(countTimelines(diagram));
}
