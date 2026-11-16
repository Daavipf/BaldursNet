using System;
using System.Collections.Generic;

namespace BaldursNet.Presentation.ConsoleUI;

public class TabInfo
{
  public string Title { get; set; } = string.Empty;
  public List<object> Items { get; set; } = [];
  public Func<object, string> DisplaySelector { get; set; } = item => item?.ToString() ?? string.Empty;
}
