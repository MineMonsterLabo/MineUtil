using Xunit;
using MineUtil;

namespace MineUtilTest
{
    public class UnitTest1
    {
        public class TestData
        {
            public int a;
        }

        [Fact]
        public void Test1()
        {
            Assert.Equal(5.ToOption().Unwrap(), new Option<int>(5).Unwrap());

            TestData testData = null;

            Assert.True(testData.ToOption().IsNone);

            Assert.Equal(55, testData.ToOption().UnwrapOr(new TestData { a = 55 }).a);

            Assert.Equal(400, 404.ToOption().Select(v => v - 4).Unwrap());

            Assert.Null(testData.ToOption().Select(v => v).UnwrapOr(null));

            Assert.Equal("hoge", 48.ToOption().Bind(v => "hoge".ToOption()).Unwrap());

            var option1 =
                from a in 88.ToOption()
                from b in 20.ToOption()
                select a + b;

            Assert.Equal(108, option1.Unwrap());

            var option2 =
                from a in 88.ToOption()
                from b in new Option<int>()
                select a + b;

            Assert.True(option2.IsNone);
        }
    }
}
