namespace BaldursNet.Presentation.ConsoleUI;

public class TabInfo
{
  public string Title { get; set; }
  public List<object> Items { get; set; }
  public Func<object, string> DisplaySelector { get; set; }
}