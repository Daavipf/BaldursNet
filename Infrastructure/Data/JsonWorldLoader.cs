using BaldursNet.Application.Interfaces;

namespace BaldursNet;

public class JsonWorldLoader : BaldursNet.Infrastructure.Data.WorldLoader
{
  public JsonWorldLoader(string? filePath = null, IRoomFactory? roomFactory = null)
    : base(filePath)
  {
  }
}
