using Xunit;

namespace SkriptInsight.Tests.Inspections
{
    public class SkriptInspectionTests : SkriptInspectionTestsBase
    {
        [Fact]
        public void EventCantBeCancelledInspectionWorks()
        {
            AssertResource("cancelnode.sk");
        }
    }
}