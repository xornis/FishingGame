
public struct ResourceData
{
    public int current;
    public int max;
    public int delta;

    public ResourceData(int current, int max, int delta)
    {
        this.current = current;
        this.max = max;
        this.delta = delta;
    }
}

public enum ResourceType { Steps, FishingAttempts, Coins }
