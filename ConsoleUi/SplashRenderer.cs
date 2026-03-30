using Spectre.Console;

namespace WorkItems.ConsoleUi;

public static class SplashRenderer
{
    private const string SplashText =
@"   ___    ____   ___        _       ___
  / _ |  / __ \ / _ \ _    | |     / _ \
 / __ | / /_/ // // /| |/| | |__  / // /
/_/ |_| \____//____/ |__,__|____/ /____/

STARC Azure DevOps Work Item Tool";

    public static void Render()
    {
        var panel = new Panel(new Markup(EscapeMarkup(SplashText)))
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1),
            Header = new PanelHeader("STARC", Justify.Center)
        };

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    public static string EscapeMarkup(string value)
    {
        return Markup.Escape(value);
    }
}
