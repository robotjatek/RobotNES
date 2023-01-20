namespace NESCore
{
    public interface IController
    {
        void Reset();
        byte ReadNextButton(); // 0/1
    }
}
