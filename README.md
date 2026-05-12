# MeridianAssessment

.NET 8 console app for the SE engineering assessment: authenticated calls to the assessment API, paginated dataset fetch, layer-1 `content_hash` submit, and stubs for deeper layers.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Configuration

1. Open [`MeridianAssessment/MeridianClient.cs`](MeridianAssessment/MeridianClient.cs).
2. Set **`baseUrl`** to your invitation **BASE_URL** (no trailing slash required for how URLs are built).
3. Set **`apiKey`** to your **`API_KEY`** (`sa_…`). Treat it as a secret; prefer environment or user secrets instead of committing real keys to git.

The client sends **`Authorization: Bearer <apiKey>`** on every request and uses a **10-minute** `HttpClient` timeout.

## Build and run

From the repo root:

```bash
dotnet restore
dotnet build
dotnet run --project MeridianAssessment/MeridianAssessment.csproj
```

[`Program.cs`](MeridianAssessment/Program.cs) currently runs **`LevelOneHandler`** as a sample flow.

## API surface (as wired in code)

| Operation | Method | Path (relative to `baseUrl`) |
|-----------|--------|------------------------------|
| Dataset page | `GET` | `/api/v1/dataset?page={n}&page_size={size}` |
| Submit | `POST` | `/api/v1/submit` |
| Secret key | `GET` | `/secret` |

The assessment guide also mentions **`GET …/api/v1/health`** (unauthenticated); it is not called from this sample client yet.

## JSON and models

- **`MeridianClient.ApiJsonOptions`**: camelCase property names, case-insensitive deserialization (matches `{ "type", "value", "notes" }` on submit).
- **[`RequestPayload`](MeridianAssessment/Models/RequestPayload.cs)**: `Type` / `Value` / `Notes` for submits.
- **[`DatasetPageResponse`](MeridianAssessment/Models/DatasetPageResponse.cs)**: paginated dataset envelope (`data`, `has_more`, `page`, `page_size`, `total`).
- **[`SubmitResponse`](MeridianAssessment/Models/SubmitResponse.cs)**: submit grading envelope (`correct`, `layer`, `message`, `submission_id`, `type`).

## Submit `type` values

Use the strings returned in **`valid_types`** on a bad submit (see [`LayerTypeConstants`](MeridianAssessment/Constants/LayerTypeConstants.cs)), for example:

- **`content_hash`** — layer 1 integrity proof
- **`decrypted_hash`**, **`algorithm_answer`**, **`analysis`**, **`repo`**, **`transcript`**

Submit **`repo`** (repository URL) **last** when you finish, per the candidate guide.

## `MeridianClient` behavior

- **`GetSampleDataSetAsync(page, pageSize)`** — returns a parsed [`DatasetPageResponse`](MeridianAssessment/Models/DatasetPageResponse.cs) (implemented via raw page bytes + JSON parse).
- **`GetSampleDataSetPageBytesAsync(page, pageSize)`** — raw UTF-8 body of one dataset page (for experiments or alternate hashing).
- **`SubmitTask(RequestPayload)`** — POST JSON to **`/api/v1/submit`**, returns **`SubmitResponse?`**.
- **`GetSecretKey()`** — GET **`/secret`**, key as JSON string.
- **429 handling**: reads **`Retry-After`**, bounded retries (see `MaxRateLimitRetryAttempts` in client).

## Layer 1 — [`LevelOneHandler`](MeridianAssessment/Handlers/LevelOneHandler.cs)

1. Loads all pages with **`hasMore`** until the dataset is complete (default **`page_size`** comes from [`GetSampleDataSetAsync`](MeridianAssessment/MeridianClient.cs) defaults, currently **100** unless you change them).
2. Caches the merged ciphertext strings under **`AppContext.BaseDirectory`** as **`SampleDataSet.json`** (see [`FilePathConstants.SampleDataSetFilePath`](MeridianAssessment/Constants/FilePathConstants.cs)).
3. Submits **`type: content_hash`** with **`value`** = **SHA-256** digest as **hex** over UTF-8 bytes of a **minified JSON array** of all ciphertext strings (see `SubmitContentHashAsync` for `JsonSerializerOptions`, including relaxed Unicode escaping).

If you change how the hash is computed, delete the cached **`SampleDataSet.json`** in the output directory so the next run refetches.

## Other handlers

[`LevelTwoHandler`](MeridianAssessment/Handlers/LevelTwoHandler.cs), [`LevelThreeHandler`](MeridianAssessment/Handlers/LevelThreeHandler.cs), and [`LevelFourHandler`](MeridianAssessment/Handlers/LevelFourHandler.cs) are **stubs** (`NotImplementedException` or empty) until you wire decryption, search, and analysis.

## Assessment reminders

- **Clock**: your window starts on the **first authenticated** request.
- **Submits are not idempotent** — every attempt is recorded; avoid noisy retry loops on **`/submit`**.
- **`notes`** on submit is optional and must stay **≤ 8 KiB** if you use it.
