namespace Saga.Shared.Definitions
{
    public interface IArtificialIntelligence
    {
        void Process();

        bool IsActivatedOnDemand { get; }

        Tasks.LifespanAI.Lifespan Lifespan { get; }
    }
}