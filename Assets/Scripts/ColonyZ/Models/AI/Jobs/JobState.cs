namespace ColonyZ.Models.AI.Jobs
{
    public enum JobState
    {
        Active, // Has been assigned to an entity.
        Idle, // Waiting to be assigned to an entity when one is available.
        Error // No entities can complete the job.
    }
}