import fs from "fs";
import readline from "readline";
import getFilePath from "../inputFiles/pathHelper.ts";

type Range = [number, number];

interface PantryInventory {
  freshRanges: Range[];
  inventory: number[];
}

export default async function Puzzle5() {
  const getPantry = async (): Promise<PantryInventory> => {
    const path = getFilePath("input-5");
    const stream = fs.createReadStream(path);

    const rl = readline.createInterface({
      input: stream,
      crlfDelay: Infinity,
    });

    const ranges: Range[] = [];
    for await (const line of rl) {
      if (line === "") {
        break;
      }

      const [start, end] = line.split("-", 2);
      ranges.push([parseInt(start), parseInt(end)]);
    }

    const inventory: number[] = [];
    for await (const line of rl) {
      inventory.push(parseInt(line));
    }

    return {
      freshRanges: ranges,
      inventory,
    };
  };

  const getFreshIngredients = (pantry: PantryInventory): number => {
    let count = 0;

    for (const ingredient of pantry.inventory) {
      for (const [start, end] of pantry.freshRanges) {
        if (ingredient >= start && ingredient <= end) {
          count++;
          break;
        }
      }
    }

    return count;
  };

  const getPossibleFreshIngredients = (pantry: PantryInventory): number => {
    const sortedRanges = pantry.freshRanges.toSorted(
      ([left], [right]) => left - right,
    );
    let [previousStart, previousEnd] = sortedRanges[0];
    let count = 0;

    for (let i = 1; i < sortedRanges.length; i++) {
      const [start, end] = sortedRanges[i];

      if (start > previousEnd) {
        count += previousEnd - previousStart + 1;
        previousStart = start;
      }

      previousEnd = Math.max(end, previousEnd);
    }

    return count + (previousEnd - previousStart) + 1;
  };

  const pantry = await getPantry();
  console.log(getFreshIngredients(pantry));
  console.log(getPossibleFreshIngredients(pantry));
}
