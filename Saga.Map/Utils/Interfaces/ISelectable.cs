namespace Saga.Shared.Definitions
{
    /// <summary>
    /// Interface that shows you can select the current actor
    /// </summary>
    public interface ISelectAble
    {
        /// <summary>
        /// Get's or Set's the HP of the actor
        /// </summary>
        ushort HP
        {
            get;
            set;
        }

        /// <summary>
        /// Get's or Set's the Maximum HP of the character
        /// </summary>
        ushort HPMAX
        {
            get;
            set;
        }

        /// <summary>
        /// Get's or Set's the SP of the actor
        /// </summary>
        ushort SP
        {
            get;
            set;
        }

        /// <summary>
        /// Get's or Set's the Maximum SP of the actor
        /// </summary>
        ushort SPMAX
        {
            get;
            set;
        }
    }
}