import fs, { ReadStream } from "fs";
import readline from "readline";
import getFilePath from "../inputFiles/pathHelper.ts";

interface MathProblem {
  operation: "+" | "*";
  values: number[];
}

type OperationFunction = (left: number, right: number) => number;

export default async function Puzzle6() {
  const getHomeworkFile = (): {
    stream: ReadStream;
    rl: readline.Interface;
  } => {
    const path = getFilePath("input-6");
    const stream = fs.createReadStream(path);
    const rl = readline.createInterface({
      input: stream,
      crlfDelay: Infinity,
    });

    return { stream, rl };
  };

  const readHomework = async (): Promise<MathProblem[]> => {
    const { stream, rl } = getHomeworkFile();
    const problems: MathProblem[] = [];

    for await (const line of rl) {
      for (const [index, part] of line
        .split(/ +/)
        .filter((str) => str.length !== 0)
        .entries()) {
        const problem = problems.at(index);

        if (problem === undefined) {
          problems[index] = {
            operation: "+",
            values: [parseInt(part)],
          };
        } else {
          const value = parseInt(part);

          if (Number.isNaN(value)) {
            if (part === "*") {
              problem.operation = part;
            }
          } else {
            problem.values.push(parseInt(part));
          }
        }
      }
    }

    rl.close();
    stream.close();
    return problems;
  };

  const addDigitToValue = (
    value: number | undefined,
    digit: number,
  ): number => {
    return (value ?? 0) * 10 + digit;
  };

  const readHomeworkColumnWise = async (): Promise<MathProblem[]> => {
    const { stream, rl } = getHomeworkFile();
    const problems: MathProblem[] = [];
    const valueColumns: number[] = [];

    for await (const line of rl) {
      let currentColumn = 0;
      for (let index = 0; index < line.length; index++) {
        const char = line.charAt(index);

        if (char === " ") {
          const previousChar = line[Math.max(0, index - 1)];
          if (previousChar !== " ") {
            currentColumn++;
          }

          continue;
        }

        const digit = parseInt(char);
        if (!Number.isNaN(digit)) {
          valueColumns[index] = addDigitToValue(valueColumns.at(index), digit);
        } else if (char === "+" || char === "*") {
          const values: number[] = [];
          for (
            let valueIndex = index;
            valueColumns.at(valueIndex) !== undefined;
            valueIndex++
          ) {
            values.push(valueColumns[valueIndex]);
          }

          problems[currentColumn] = {
            operation: char,
            values,
          };
        }
      }
    }

    stream.close();
    rl.close();
    return problems;
  };

  const getOperationFunction = (operation: "+" | "*"): OperationFunction => {
    switch (operation) {
      case "+":
        return (left, right) => left + right;
      case "*":
        return (left, right) => left * right;
    }
  };

  const solveHomework = (homework: readonly MathProblem[]): number => {
    return homework.reduce(
      (sum, problem) =>
        sum +
        problem.values.reduce((previous, current) =>
          getOperationFunction(problem.operation)(previous, current),
        ),
      0,
    );
  };

  const homework = await readHomework();
  console.log(solveHomework(homework));

  const columnHomework = await readHomeworkColumnWise();
  console.log(solveHomework(columnHomework));
}
