# MeridianAssessment

.NET 8 console project that calls the Meridian API via `MeridianClient` (`IMeridianClient`).

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Initial create-client steps

1. **Restore and build** (from the repo root):

   ```bash
   dotnet restore
   dotnet build
   ```

2. **Create the client** in your entry code (for example `Program.cs`):

   ```csharp
   using MeridianAssessment;
   using MeridianAssessment.Interfaces;

   IMeridianClient client = new MeridianClient();
   ```

3. **Configure the API** before making calls. In the default `MeridianClient` implementation, set the private `baseUrl` and `apiKey` fields to your Meridian API root URL and bearer token so `HttpClient` sends `Authorization: Bearer …` and requests hit the correct host.

4. **Call the client** as needed, for example:

   - `await client.GetSecretKey()` — GET `{baseUrl}/secret` and deserialize the key from JSON.
   - `await client.GetSampleDataSet()` — loads sample data (cached in `SampleDataSet.json` when fetched from the API).
   - `await client.SubmitTask(payload)` — submits a `RequestPayload` to `{baseUrl}/submit`.

5. **Run the app** (once `Program.cs` invokes the client):

   ```bash
   dotnet run --project MeridianAssessment
   ```
