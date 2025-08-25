using System.IO;

public class NpoiMemoryStream : MemoryStream
{
    public bool AllowClose { get; set; }
    public NpoiMemoryStream()
    {
        AllowClose = true;
    }

    public override void Close()
    {
        if (AllowClose)
            base.Close();
    }
}