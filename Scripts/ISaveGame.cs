namespace LibYiroth.Save
{
    public interface ISaveGame
    {
        bool OnSaving(SaveManager saveManager);
        bool OnLoading(SaveManager saveManager);
    }
}
