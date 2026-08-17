namespace BaldursNet.Presentation.ConsoleUI;

public class SelectionMenuParams<T>(
  List<T> items,
  string title,
  string? description = null,
  Func<T, string>? displaySelector = null,
  bool canCancel = true,
  string prompt = "\n[↑/↓] Alternar  |  [Enter] Selecionar")
{
  public List<T> Items { get; } = items;
  public string Title { get; } = title;
  public string? Description { get; } = description;
  public Func<T, string>? DisplaySelector { get; set; } = displaySelector;
  public bool CanCancel { get; } = canCancel;
  public string Prompt { get; } = prompt;
}