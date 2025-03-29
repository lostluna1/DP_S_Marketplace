namespace DP_S_Marketplace.Models;
public class GlobalVariables
{
    private static readonly object _lock = new();

    private GlobalVariables()
    {
    }

    public static GlobalVariables Instance
    {
        get
        {
            if (field == null)
            {
                lock (_lock)
                {
                    field ??= new GlobalVariables();
                }
            }
            return field;
        }
    }


    public ConnectionInfo? ConnectionInfo
    {
        get; set;
    }
}
