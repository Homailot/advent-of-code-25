import path, { dirname } from "node:path";
import { fileURLToPath } from "node:url";

export default function getFilePath(fileName: string): string {
  const currentFile = fileURLToPath(import.meta.url);
  const inputDirectory = dirname(currentFile);

  return path.join(inputDirectory, fileName);
}
