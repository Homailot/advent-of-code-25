import js from "@eslint/js";
import importPlugin from "eslint-plugin-import";
import { defineConfig } from "eslint/config";
import globals from "globals";
import tseslint from "typescript-eslint";

export default defineConfig(
  { ignores: ["dist", "node_modules/**"] },
  {
    extends: [
      js.configs.recommended,
      ...tseslint.configs.recommendedTypeChecked,
      ...tseslint.configs.stylisticTypeChecked,
    ],
    files: ["**/*.{ts,tsx}"],
    languageOptions: {
      parser: tseslint.parser,
      ecmaVersion: "latest",
      sourceType: "module",
      globals: {
        ...globals.node,
        ...globals.es2025,
      },
      parserOptions: {
        project: "./tsconfig.json",
      },
    },
    plugins: {
      import: importPlugin,
      "@typescript-eslint": tseslint.plugin,
    },
    rules: {
      ...importPlugin.configs.recommended.rules,
      ...importPlugin.configs.typescript.rules,

      // General rules
      "linebreak-style": ["error", "unix"],
    },
    settings: {
      "import/resolver": {
        typescript: {
          alwaysTryTypes: true, // Always try to resolve types under `<root>@types` directory even if it doesn't contain any source code, like `@types/unist`
        },
      },
    },
  },
);
