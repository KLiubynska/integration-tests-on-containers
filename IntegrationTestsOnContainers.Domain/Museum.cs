namespace IntegrationTestsOnContainers.Domain;

public class Museum
{
    public Museum(string name, bool isOpened)
    {
        Name = name;
        IsOpened = isOpened;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }  
    
    public bool IsOpened { get; private set; }

    public void Open()
    {
        IsOpened  = true;
    }

    public void Close()
    {
        IsOpened = false;
    }

    private Museum()
    {
    }
}
