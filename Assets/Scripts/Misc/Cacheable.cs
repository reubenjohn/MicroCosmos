using System;

public class Cacheable<T>
{
    private readonly Func<T> onRefresh;
    private T cached;
    private bool refreshRequired = true;

    public Cacheable(Func<T> onRefresh)
    {
        this.onRefresh = onRefresh;
    }

    public Cacheable(Func<T> onRefresh, T initial)
    {
        this.onRefresh = onRefresh;
        cached = initial;
        refreshRequired = false;
    }

    public T Value
    {
        get
        {
            if (refreshRequired)
            {
                cached = onRefresh();
                refreshRequired = false;
            }

            return cached;
        }
    }

    public void Invalidate() => refreshRequired = true;
}