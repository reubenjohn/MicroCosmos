using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class MutationTest
    {
        [Test]
        public void SignedIntMutation()
        {
            var mutator = new Mutator.Int(0.2f);

            Random.InitState(1); // sample ~= -0.99
            Assert.AreEqual(-1, mutator.Mutate(0));

            Random.InitState(0); // sample ~= -0.16
            Assert.AreEqual(-2, mutator.Mutate(-2));

            Random.InitState(2); // sample ~=  0.17
            Assert.AreEqual(0, mutator.Mutate(0));

            Random.InitState(18); // sample ~=  0.903
            Assert.AreEqual(0, mutator.Mutate(-1));
        }

        [Test]
        public void UnsignedIntMutation()
        {
            var mutator = new Mutator.UnsignedInt(0.2f);

            Random.InitState(1); // sample ~= -0.99
            Assert.AreEqual(0, mutator.Mutate(0));

            Random.InitState(0); // sample ~= -0.16
            Assert.AreEqual(0, mutator.Mutate(0));

            Random.InitState(2); // sample ~=  0.17
            Assert.AreEqual(1, mutator.Mutate(1));

            Random.InitState(18); // sample ~=  0.903
            Assert.AreEqual(13, mutator.Mutate(12));
        }

        [Test]
        public void EnumMutation()
        {
            var mutator = new Mutator.Enum(0.2f);

            Random.InitState(1); // sample ~= -0.99
            Assert.AreEqual(MyEnum.A, mutator.Mutate(MyEnum.A));

            Random.InitState(1); // sample ~= -0.99
            Assert.AreEqual(MyEnum.A, mutator.Mutate(MyEnum.B));

            Random.InitState(0); // sample ~= -0.16
            Assert.AreEqual(MyEnum.B, mutator.Mutate(MyEnum.B));

            Random.InitState(2); // sample ~=  0.17
            Assert.AreEqual(MyEnum.B, mutator.Mutate(MyEnum.B));

            Random.InitState(18); // sample ~=  0.903
            Assert.AreEqual(MyEnum.C, mutator.Mutate(MyEnum.B));

            Random.InitState(18); // sample ~=  0.903
            Assert.AreEqual(MyEnum.C, mutator.Mutate(MyEnum.C));
        }

        [Test]
        public void EnumTMutation()
        {
            var mutator = new Mutator.Enum<MyEnum>(0.2f);

            Random.InitState(1); // sample ~= -0.99
            Assert.AreEqual(MyEnum.A, mutator.Mutate(MyEnum.A));

            Random.InitState(1); // sample ~= -0.99
            Assert.AreEqual(MyEnum.A, mutator.Mutate(MyEnum.B));

            Random.InitState(0); // sample ~= -0.16
            Assert.AreEqual(MyEnum.B, mutator.Mutate(MyEnum.B));

            Random.InitState(2); // sample ~=  0.17
            Assert.AreEqual(MyEnum.B, mutator.Mutate(MyEnum.B));

            Random.InitState(18); // sample ~=  0.903
            Assert.AreEqual(MyEnum.C, mutator.Mutate(MyEnum.B));

            Random.InitState(18); // sample ~=  0.903
            Assert.AreEqual(MyEnum.C, mutator.Mutate(MyEnum.C));
        }

        [Test]
        public void MutationEnumerableMutation()
        {
            var elemMutator = new Mutator.Int(0.2f);
            var mutator = new Mutator.MutationEnumerable<int>(elem => elemMutator.Mutate(elem));

            TestEnumerableMutation(mutator);
        }

        [Test]
        public void MutatorEnumerableMutation()
        {
            var mutator = new Mutator.MutatorEnumerable<int>(new Mutator.Int(0.2f));

            TestEnumerableMutation(mutator);
        }

        private void TestEnumerableMutation(Mutator.MutationEnumerable<int> mutator)
        {
            Random.InitState(1); // sample ~= -0.99
            Assert.AreEqual(new[] {-1}, mutator.Mutate(new[] {0}));

            Random.InitState(0); // sample ~= -0.16
            Assert.AreEqual(new[] {-2}, mutator.Mutate(new[] {-2}));

            Random.InitState(2); // sample ~=  0.17
            Assert.AreEqual(new[] {0}, mutator.Mutate(new[] {0}));

            Random.InitState(18); // sample ~=  0.903
            Assert.AreEqual(new[] {0}, mutator.Mutate(new[] {-1}));
        }

        private enum MyEnum
        {
            A,
            B,
            C
        }
    }
}