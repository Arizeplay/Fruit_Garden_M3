public class RewardItem
{
    public ItemID ItemID = ItemID.complexity;
    public int Count;

    public void Obtain()
    {
        CurrentUser.main[ItemID] += Count;
    }
}