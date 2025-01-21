
using Ardalis.SmartEnum;

namespace Infrastructure.Enum
{
    public sealed class TypeOperationEnum: SmartEnum<TypeOperationEnum>
    {
        private TypeOperationEnum(string name, int value): base(name, value) { }

        public static readonly TypeOperationEnum Insert = new TypeOperationEnum(nameof(Insert), 1);
        public static readonly TypeOperationEnum Update = new TypeOperationEnum(nameof(Update), 2);
        public static readonly TypeOperationEnum Delete = new TypeOperationEnum(nameof(Delete), 3);
    }
}
