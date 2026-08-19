import fs from "node:fs/promises";
import { FileBlob, SpreadsheetFile } from "@oai/artifact-tool";
const input = await FileBlob.load("C:/バックエンド/ReservationManagerAPI2/outputs/ReservationManagerAPI2_学習ステップ.xlsx");
const workbook = await SpreadsheetFile.importXlsx(input);
const preview = await workbook.render({ sheetName: "最終構成", range: "A1:E14", scale: 1.2, format: "png" });
await fs.writeFile("C:/バックエンド/ReservationManagerAPI2/outputs/ReservationManagerAPI2_最終構成.png", new Uint8Array(await preview.arrayBuffer()));
