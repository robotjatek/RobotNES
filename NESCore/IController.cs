namespace NESCore
{
    public interface IController
    {
        public enum Keys
        {
            A = 0b1,
            B = 0b10,
            SELECT = 0b100,
            START = 0b1000,
            UP = 0b10000,
            DOWN = 0b100000,
            LEFT = 0b1000000,
            RIGHT = 0b10000000,
        }
        
        void StrobeWrite(byte value);
        byte ReadNextButton(); // 0/1
        void PressKey(Keys key);
        void ReleaseKey(Keys key);
    }
}
