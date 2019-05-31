using System.Collections.Generic;

namespace VkCelebrationScheduler
{
    public class SearchParams
    {
        public ushort? AgeFrom { get; set; } = 18;

        public ushort? AgeTo { get; set; } = 28;

        public LastSeenMode LastSeenMode { get; set; } = LastSeenMode.Online;

        public Sex Sex { get; set; } = Sex.Female;

        public IEnumerable<RelationType> RelationTypes { get; set; } = new[]
        {
            RelationType.NotMarried,
            RelationType.InActiveSearch
        };


        public long? CityId { get; set; }

        public long? UniversityId { get; set; }

        public bool IsOpened { get; set; } = true;
    }

    public enum LastSeenMode
    {
        Online = 0,
        Last24Hours = 1
    }

    public enum RelationType
    {
        Unknown = 0,
        NotMarried = 1,
        HasFriend = 2,
        Engaged = 3,
        Married = 4,
        ItsComplex = 5,
        InActiveSearch = 6,
        Amorous = 7,
        CivilMarriage = 8
    }

    public enum Sex
    {
        All = 0,
        Female = 1,
        Male = 2
    }
}
