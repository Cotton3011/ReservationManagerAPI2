# ReservationManagerAPI2

予約管理システムを想定した、ASP.NET Core Web API の学習用プロジェクトです。

ユーザーはログイン後に自分の予約を作成・確認・キャンセルできます。  
管理者は全ユーザーの予約を確認し、必要に応じてキャンセルできます。

## 学習目的

- JWT 認証
- User / Admin の認可
- 日時の扱い
- 予約の重複判定
- 予約状態の管理
- Service 層での業務ロジック
- Entity Framework Core

## 主な機能

### 認証

- ユーザー登録
- パスワードのハッシュ化
- ログイン
- JWT 発行
- ログインユーザーの UserId / Role 取得

### 予約管理

- 予約作成
- 自分の予約一覧取得
- 自分の予約詳細取得
- 自分の予約キャンセル
- 過去日時の予約防止
- 開始日時と終了日時のバリデーション
- 予約の重複防止
- Cancelled 状態の予約を重複判定から除外

### 管理者機能

- 全ユーザーの予約一覧取得
- 任意の予約キャンセル

## Entity 構成

### User

- Id
- UserName
- PasswordHash
- Role
- CreateTime

### Reservation

- Id
- UserId
- StartTime
- EndTime
- Memo
- Status
- CreateAt
- UpdateAt

## API 予定

### Auth

- `POST /api/auth/register`
- `POST /api/auth/login`

### Reservations

- `POST /api/reservations`
- `GET /api/reservations`
- `GET /api/reservations/{id}`
- `PATCH /api/reservations/{id}/cancel`

### Admin

- `GET /api/admin/reservations`
- `PATCH /api/admin/reservations/{id}/cancel`

## 実装ステップ

1. Entity / Enum を作成する
2. DbContext を作成する
3. EF Core の設定とマイグレーションを行う
4. ユーザー登録処理を作成する
5. ログイン処理と JWT 発行を作成する
6. JWT 認証を設定する
7. 予約作成処理を作成する
8. 予約日時のバリデーションを追加する
9. 予約重複判定を追加する
10. 自分の予約一覧・詳細取得を作成する
11. 予約キャンセル処理を作成する
12. Admin 用の予約管理 API を作成する
13. 動作確認とエラー処理を整理する

## 予約重複判定

既存予約と新規予約が重なる場合は予約不可にします。

```csharp
existingReservation.StartTime < newReservation.EndTime
    && existingReservation.EndTime > newReservation.StartTime
```

ただし、`Cancelled` 状態の予約は重複判定の対象外にします。
