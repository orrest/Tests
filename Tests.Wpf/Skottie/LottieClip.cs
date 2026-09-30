using System.IO;

namespace Tests.Wpf.Skottie;

public class LottieClip
{
    public required string Title { get; init; }

    public required string FileName { get; init; }

    // Sample JSON files live in <output>\Files, copied by the csproj rule (Files\*.*).
    // 示例 JSON 位于 <输出目录>\Files，由 csproj 规则（Files\*.*）复制。
    public string FilePath => Path.Combine(AppContext.BaseDirectory, "Files", FileName);

    public override string ToString()
    {
        return Title;
    }
}
