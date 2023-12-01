namespace SimpleOpenAi.Core;

public record MultipartRequestFile(Stream Stream, string Name)
{
    public Stream Stream { get; } = Stream;
    public string Name { get; } = Name;
}