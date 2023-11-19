# Image generations

Provides an implementation of OpenAI API's image generations.
https://platform.openai.com/docs/api-reference/images

Differences from OpenAI doc:
- `response_format` is decided by `CreateUrlAsync` or `CreateBytesAsync`
- Shortcuts for results
  - `urlResult.Data[0].Url` can be replaced with `urlResult.Url` or `urlResult.GetUrl(0)`
  - `base64Result.Data[0].Bytes` can be replaced with `base64Result.Bytes` or `urlResult.GetBytes(0)`

## Examples:
TODO