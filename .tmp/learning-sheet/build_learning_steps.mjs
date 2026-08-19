import fs from "node:fs/promises";
import { SpreadsheetFile, Workbook } from "@oai/artifact-tool";

const outputDir = "C:/バックエンド/ReservationManagerAPI2/outputs";
await fs.mkdir(outputDir, { recursive: true });

const workbook = Workbook.create();
const stepsSheet = workbook.worksheets.add("学習ステップ");
const architectureSheet = workbook.worksheets.add("最終構成");

const titleFormat = {
  fill: "#0F4C5C",
  font: { bold: true, color: "#FFFFFF", size: 16 },
  horizontalAlignment: "left",
  verticalAlignment: "center",
};
const sectionFormat = {
  fill: "#DCEEF2",
  font: { bold: true, color: "#12343B" },
  horizontalAlignment: "left",
  verticalAlignment: "center",
};
const headerFormat = {
  fill: "#176B87",
  font: { bold: true, color: "#FFFFFF" },
  horizontalAlignment: "center",
  verticalAlignment: "center",
  wrapText: true,
};
const bodyBorder = { preset: "all", style: "thin", color: "#D8E1E5" };

stepsSheet.showGridLines = false;
stepsSheet.mergeCells("A1:G1");
stepsSheet.getRange("A1").values = [["ReservationManagerAPI2 学習・実装ステップ一覧"]];
stepsSheet.getRange("A1:G1").format = titleFormat;
stepsSheet.getRange("A1:G1").format.rowHeight = 28;

stepsSheet.mergeCells("A2:G2");
stepsSheet.getRange("A2").values = [["API設計、Docker、CI/CD、Azure公開・監視までの学習内容と確認結果を記録した一覧です。"]];
stepsSheet.getRange("A2:G2").format = {
  fill: "#EDF5F7",
  font: { color: "#35545D", italic: true },
  verticalAlignment: "center",
};
stepsSheet.getRange("A2:G2").format.rowHeight = 22;

stepsSheet.getRange("A4:C4").values = [["分類", "完了ステップ数", "主な到達点"]];
stepsSheet.getRange("A4:C4").format = headerFormat;
stepsSheet.getRange("A5:C7").values = [
  ["API基礎", null, "JWT認証・認可・予約管理・例外処理・ログ"],
  ["Docker / CI", null, "Compose・環境変数・自動build / test"],
  ["Azure", null, "Azure SQL・Container Apps・監視・CI/CD"],
];
stepsSheet.getRange("B5").formulas = [["=COUNTIF($A$11:$A$36,A5)"]];
stepsSheet.getRange("B5:B7").fillDown();
stepsSheet.getRange("A4:C7").format.borders = bodyBorder;
stepsSheet.getRange("A5:A7").format.font = { bold: true, color: "#12343B" };
stepsSheet.getRange("B5:B7").format = {
  fill: "#E7F5EC",
  font: { bold: true, color: "#1D6B3A" },
  horizontalAlignment: "center",
};

stepsSheet.getRange("E4:G4").values = [["全体進捗", "完了", "備考"]];
stepsSheet.getRange("E4:G4").format = headerFormat;
stepsSheet.getRange("E5:G5").values = [["記録したステップ", null, "すべて動作確認済み"]];
stepsSheet.getRange("F5").formulas = [["=COUNTIF($G$11:$G$36,\"完了\")"]];
stepsSheet.getRange("E4:G5").format.borders = bodyBorder;
stepsSheet.getRange("F5").format = {
  fill: "#E7F5EC",
  font: { bold: true, color: "#1D6B3A" },
  horizontalAlignment: "center",
};

const stepHeaders = [["分類", "Step", "テーマ", "目的・学んだこと", "主な実装・設定", "確認結果", "状態"]];
const stepRows = [
  ["API基礎", "1", "Entity と Role enum", "User と Reservation のデータ構造、User / Admin の役割を定義する。", "User、Reservation、UserRole、ReservationStatus を作成。", "Entity と enum を作成し、責務を整理。", "完了"],
  ["API基礎", "2", "EF Core と Migration", "DbContext とテーブル定義、Migration の役割を理解する。", "AppDbContext、OnModelCreating、初回 Migration を作成。", "SQL Server に Users / Reservations テーブルを作成。", "完了"],
  ["API基礎", "3", "認証サービス", "登録・ログインの業務ロジックを Service 層へ分離する。", "Request DTO、AuthService、PasswordHasher を実装。", "ユーザー登録とログインを確認。", "完了"],
  ["API基礎", "4", "JWT 認証と認可", "ログイン後に JWT を発行し、User / Admin を認可する。", "JwtService、AddAuthentication、Authorize、Swagger Bearer 認証を設定。", "401 / 403 とロール認可を確認。", "完了"],
  ["API基礎", "5", "予約管理", "日時検証、重複判定、キャンセル状態を業務ルールとして実装する。", "ReservationService、CreateReservationRequest、重複判定を実装。", "予約作成、一覧、詳細、キャンセルを確認。", "完了"],
  ["API基礎", "6", "Admin 機能", "管理者だけが全予約を閲覧・キャンセルできるようにする。", "AdminController と Admin 用 Service 処理を実装。", "Admin の全件取得と任意キャンセルを確認。", "完了"],
  ["API基礎", "7", "例外・ログ・単体テスト", "API の失敗を適切な HTTP 応答にし、業務イベントを記録する。", "ExceptionHandlingMiddleware、ILogger、xUnit テストを追加。", "400 / 404 / 409 と ReservationService テストを確認。", "完了"],
  ["Docker / CI", "1", "Docker 基礎", "Image、Container、Dockerfile、Compose、volume、environment を理解する。", "Docker の基本概念とローカル構成を整理。", "Image と Container の違いを確認。", "完了"],
  ["Docker / CI", "2", "API のコンテナ化", ".NET 8 API を Dockerfile でビルド・実行する。", "マルチステージ Dockerfile、.dockerignore を作成。", "docker build / docker run と Swagger 表示を確認。", "完了"],
  ["Docker / CI", "3", "SQL Server コンテナ", "ローカル DB をコンテナで起動し、ポートと永続化を理解する。", "SQL Server 2022 コンテナ、1433 ポートを使用。", "docker ps と Docker Desktop で起動を確認。", "完了"],
  ["Docker / CI", "4", "Docker Compose", "API と DB を1つの構成として起動する。", "docker-compose.yml、depends_on、healthcheck、volume を設定。", "API から db サービス名で接続できることを確認。", "完了"],
  ["Docker / CI", "5", "環境変数管理", "接続文字列と JWT 設定をコード外から渡す。", ".env、.env.example、環境変数キーを設定。", "秘密情報を Git 管理しない構成を確認。", "完了"],
  ["Docker / CI", "6", "GitHub Actions build", "push 時にクラウド上でビルドを検証する。", ".github/workflows/ci.yml、restore / build を設定。", "GitHub Actions の build 成功を確認。", "完了"],
  ["Docker / CI", "7", "GitHub Actions test", "テスト失敗時に品質問題を検出できるようにする。", "tests プロジェクトを整理し、dotnet test を CI へ追加。", "xUnit テスト成功を確認。", "完了"],
  ["Docker / CI", "8", "例外処理 Middleware", "Controller / Service の例外を HTTP 応答へ一元変換する。", "ConflictException、NotFoundException、ProblemDetails を実装。", "Swagger で 400 / 404 / 409 を確認。", "完了"],
  ["Docker / CI", "9", "ILogger", "予約作成・重複・キャンセル・例外を追跡できるようにする。", "ReservationService と Middleware に ILogger を追加。", "Docker Compose のログで業務ログを確認。", "完了"],
  ["Docker / CI", "10", "README 整理", "ローカル起動・API・ER図・運用情報を共有する。", "Docker、CI、環境変数、API一覧、ER図を記載。", "README から環境を再現できる形に整理。", "完了"],
  ["Azure", "1", "Azure 基礎", "サブスクリプション、リソースグループ、リージョンの役割を理解する。", "Azure Portal と Japan East の構成を確認。", "学習用リソースの配置先を決定。", "完了"],
  ["Azure", "2", "コスト管理", "学習中の想定外課金を抑える設定を確認する。", "予算、無料プラン、超過請求無効、スケール0を設定。", "Azure SQL の無料枠とコスト画面を確認。", "完了"],
  ["Azure", "3", "Azure SQL Database", "クラウド DB を作成し、接続先をローカルから切り替える。", "サーバーレス Azure SQL Database と SQL 認証を作成。", "Azure Portal のクエリエディターでテーブルを確認。", "完了"],
  ["Azure", "4", "Azure SQL Migration", "EF Core Migration を Azure SQL Database へ適用する。", "User Secrets の接続文字列で dotnet ef database update を実行。", "Migration 履歴とテーブル作成を確認。", "完了"],
  ["Azure", "5", "Docker Hub", "Container Apps が取得できる公開イメージを管理する。", "Docker Hub リポジトリ、タグ、push を設定。", "latest と otel-v1 イメージを確認。", "完了"],
  ["Azure", "6", "Container Apps 公開", "Docker イメージを HTTPS の公開 API として実行する。", "Container Apps、環境、ingress、シークレット、環境変数を設定。", "Azure の Swagger からログイン・予約を確認。", "完了"],
  ["Azure", "7", "Application Insights", "クラウド上のリクエスト、SQL 接続、アプリログを観測する。", "Azure Monitor OpenTelemetry、Application Insights、Log Analytics を設定。", "AppRequests、AppDependencies、ContainerAppConsoleLogs を確認。", "完了"],
  ["Azure", "8", "GitHub Actions CI/CD", "push 後にテスト済みイメージだけを Azure へ自動デプロイする。", "OIDC、GitHub Secrets、Docker push、az containerapp update を設定。", "build / test / deploy の自動成功を確認。", "完了"],
  ["Azure", "9", "Azure README", "公開・監視・CI/CD の再現手順をドキュメント化する。", "Azure構成図、環境変数、監視、CI/CD、コスト方針を追記。", "運用情報を README に集約。", "完了"],
];

stepsSheet.getRange("A10:G10").values = stepHeaders;
stepsSheet.getRange(`A11:G${10 + stepRows.length}`).values = stepRows;
stepsSheet.getRange(`A10:G${10 + stepRows.length}`).format.borders = bodyBorder;
stepsSheet.getRange("A10:G10").format = headerFormat;
stepsSheet.getRange(`A11:G${10 + stepRows.length}`).format.verticalAlignment = "top";
stepsSheet.getRange(`A11:G${10 + stepRows.length}`).format.wrapText = true;
stepsSheet.getRange(`G11:G${10 + stepRows.length}`).format = {
  fill: "#E7F5EC",
  font: { bold: true, color: "#1D6B3A" },
  horizontalAlignment: "center",
  verticalAlignment: "center",
};
stepsSheet.getRange(`A11:A${10 + stepRows.length}`).format.font = { bold: true, color: "#176B87" };
stepsSheet.getRange("A10:G10").format.rowHeight = 28;
stepsSheet.getRange(`A11:G${10 + stepRows.length}`).format.rowHeight = 42;
stepsSheet.getRange("A1:A40").format.columnWidth = 15;
stepsSheet.getRange("B1:B40").format.columnWidth = 8;
stepsSheet.getRange("C1:C40").format.columnWidth = 24;
stepsSheet.getRange("D1:D40").format.columnWidth = 42;
stepsSheet.getRange("E1:E40").format.columnWidth = 48;
stepsSheet.getRange("F1:F40").format.columnWidth = 38;
stepsSheet.getRange("G1:G40").format.columnWidth = 12;
stepsSheet.freezePanes.freezeRows(10);
stepsSheet.tables.add(`A10:G${10 + stepRows.length}`, true, "LearningStepsTable");

architectureSheet.showGridLines = false;
architectureSheet.mergeCells("A1:E1");
architectureSheet.getRange("A1").values = [["ReservationManagerAPI2 最終構成・運用一覧"]];
architectureSheet.getRange("A1:E1").format = titleFormat;
architectureSheet.getRange("A1:E1").format.rowHeight = 28;
architectureSheet.mergeCells("A2:E2");
architectureSheet.getRange("A2").values = [["ローカル開発、クラウド公開、監視、自動デプロイで使用した主要要素を整理しています。"]];
architectureSheet.getRange("A2:E2").format = {
  fill: "#EDF5F7",
  font: { color: "#35545D", italic: true },
  verticalAlignment: "center",
};

architectureSheet.getRange("A4:E4").values = [["領域", "要素", "役割", "主な確認方法", "秘密情報の扱い"]];
architectureSheet.getRange("A4:E4").format = headerFormat;
const architectureRows = [
  ["API", "ASP.NET Core Web API (.NET 8)", "認証・認可・予約管理 API を提供する。", "Swagger、xUnit テスト、Container Apps URL", "JWT 設定は環境変数から取得"],
  ["データ", "Entity Framework Core", "Entity と Migration を通して DB を操作する。", "Migration 履歴、テスト、SQL クエリ", "接続文字列はコードに書かない"],
  ["ローカル実行", "Docker Compose", "API と SQL Server をまとめて起動する。", "docker compose up --build、Swagger", ".env は .gitignore で除外"],
  ["ローカル DB", "SQL Server 2022 Container", "開発用データを永続化する。", "Docker Desktop、sqlserver-data volume", "sa パスワードは .env で管理"],
  ["クラウド DB", "Azure SQL Database", "公開 API のユーザー・予約データを保存する。", "Azure Portal、クエリエディター、Migration", "接続文字列は Container Apps のシークレット"],
  ["公開", "Azure Container Apps", "Docker イメージを HTTPS API として実行する。", "アプリケーション URL、リビジョン、Swagger", "シークレットを環境変数へ参照設定"],
  ["イメージ管理", "Docker Hub", "Container Apps 用の Docker イメージを配布する。", "My Hub、イメージタグ、pull", "アクセストークンは GitHub Secrets"],
  ["監視", "Application Insights", "HTTP リクエストと SQL 依存関係を可視化する。", "AppRequests、AppDependencies", "接続文字列は Container Apps のシークレット"],
  ["ログ", "Log Analytics", "ILogger とコンテナログを検索する。", "ContainerAppConsoleLogs、リビジョンログ", "パスワード・JWT・接続文字列を出力しない"],
  ["CI/CD", "GitHub Actions + OIDC", "build / test 成功時だけイメージを公開・デプロイする。", "Actions の build / deploy ジョブ", "Azure は OIDC、Docker Hub は GitHub Secrets"],
];
architectureSheet.getRange(`A5:E${4 + architectureRows.length}`).values = architectureRows;
architectureSheet.getRange(`A4:E${4 + architectureRows.length}`).format.borders = bodyBorder;
architectureSheet.getRange(`A5:E${4 + architectureRows.length}`).format.wrapText = true;
architectureSheet.getRange(`A5:E${4 + architectureRows.length}`).format.verticalAlignment = "top";
architectureSheet.getRange("A5:A14").format.font = { bold: true, color: "#176B87" };
architectureSheet.getRange("A4:E4").format.rowHeight = 28;
architectureSheet.getRange("A5:E14").format.rowHeight = 40;
architectureSheet.getRange("A1:A20").format.columnWidth = 16;
architectureSheet.getRange("B1:B20").format.columnWidth = 30;
architectureSheet.getRange("C1:C20").format.columnWidth = 42;
architectureSheet.getRange("D1:D20").format.columnWidth = 36;
architectureSheet.getRange("E1:E20").format.columnWidth = 46;
architectureSheet.freezePanes.freezeRows(4);
architectureSheet.tables.add(`A4:E${4 + architectureRows.length}`, true, "ArchitectureTable");

const inspection = await workbook.inspect({
  kind: "table",
  range: "学習ステップ!A4:G36",
  include: "values,formulas",
  tableMaxRows: 8,
  tableMaxCols: 7,
});
console.log(inspection.ndjson);

const errors = await workbook.inspect({
  kind: "match",
  searchTerm: "#REF!|#DIV/0!|#VALUE!|#NAME\\?|#N/A",
  options: { useRegex: true, maxResults: 50 },
  summary: "formula error scan",
});
console.log(errors.ndjson);

const preview = await workbook.render({
  sheetName: "学習ステップ",
  range: "A1:G36",
  scale: 1.2,
  format: "png",
});
await fs.writeFile(`${outputDir}/ReservationManagerAPI2_学習ステップ.png`, new Uint8Array(await preview.arrayBuffer()));

const output = await SpreadsheetFile.exportXlsx(workbook);
await output.save(`${outputDir}/ReservationManagerAPI2_学習ステップ.xlsx`);
