﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis.Testing;
using System;
using System.Threading.Tasks;
using Xunit;

using VerifyCS = DocumentFormat.OpenXml.Generator.Tests.OpenXmlSourceGeneratorVerifier<
    DocumentFormat.OpenXml.Generator.KnownFeaturesGenerator>;

namespace DocumentFormat.OpenXml.Generator.Tests
{
    public class KnownFeaturesGeneratorTests
    {
        private const string Attribute = @"// <auto-generated/>

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

namespace DocumentFormat.OpenXml;

[global::System.Diagnostics.Conditional(""GENERATOR"")]
[global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
internal sealed class KnownFeatureAttribute : global::System.Attribute
{
    public KnownFeatureAttribute(global::System.Type contract, global::System.Type? service = null)
    {
    }
}

[global::System.Diagnostics.Conditional(""GENERATOR"")]
[global::System.AttributeUsage(global::System.AttributeTargets.Method)]
internal sealed class ThreadSafeAttribute : global::System.Attribute
{
}
";

        [Fact]
        public async Task Empty()
        {
            var test = string.Empty;

            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { test },
                    GeneratedSources =
                    {
                        (typeof(KnownFeaturesGenerator), "KnownFeatureAttribute.cs", Attribute),
                    },
                },
            }.RunAsync();
        }

        [Fact]
        public async Task OnlyOneServiceCanBeRegistered()
        {
            var test = @"using DocumentFormat.OpenXml;

namespace Test;

public interface ITest
{
}

public class TestImpl : ITest
{
}

public class TestImpl2 : ITest
{
}

public partial class Factory
{
    [KnownFeature(typeof(ITest), typeof(TestImpl))]
    [{|#0:KnownFeature(typeof(ITest), typeof(TestImpl2))|}]
    [ThreadSafe]
    private partial T Get<T>();
}";
            var created = @"// <auto-generated/>

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

namespace Test;

public partial class Factory
{
    private readonly object _LockGet = new();

    private global::Test.ITest? _TestImpl;

    private partial T? Get<T>()
    {
        if (typeof(T) == typeof(global::Test.ITest))
        {
            if (_TestImpl is null)
            {
                lock(_LockGet)
                {
                    if (_TestImpl is null)
                    {
                        _TestImpl = new global::Test.TestImpl();
                    }
                }
            }

            return (T)_TestImpl;
        }

        return default;
    }
}
";

            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { test },
                    GeneratedSources =
                    {
                        (typeof(KnownFeaturesGenerator), "KnownFeatureAttribute.cs", Attribute),
                        (typeof(KnownFeaturesGenerator), "Factory_Get.cs", created),
                    },
                    ExpectedDiagnostics =
                    {
                        DiagnosticResult.CompilerError("OOX1001").WithLocation(0),
                    },
                },
            }.RunAsync();
        }

        [Fact]
        public async Task DuplicateKnown()
        {
            var test = @"using DocumentFormat.OpenXml;

namespace Test;

public interface ITest
{
}

public class TestImpl : ITest
{
}

public partial class Factory
{
    [KnownFeature(typeof(ITest), typeof(TestImpl))]
    [{|#0:KnownFeature(typeof(ITest), typeof(TestImpl))|}]
    [ThreadSafe]
    private partial T Get<T>();
}";
            var created = @"// <auto-generated/>

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

namespace Test;

public partial class Factory
{
    private readonly object _LockGet = new();

    private global::Test.ITest? _TestImpl;

    private partial T? Get<T>()
    {
        if (typeof(T) == typeof(global::Test.ITest))
        {
            if (_TestImpl is null)
            {
                lock(_LockGet)
                {
                    if (_TestImpl is null)
                    {
                        _TestImpl = new global::Test.TestImpl();
                    }
                }
            }

            return (T)_TestImpl;
        }

        return default;
    }
}
";

            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { test },
                    GeneratedSources =
                    {
                        (typeof(KnownFeaturesGenerator), "KnownFeatureAttribute.cs", Attribute),
                        (typeof(KnownFeaturesGenerator), "Factory_Get.cs", created),
                    },
                    ExpectedDiagnostics =
                    {
                        DiagnosticResult.CompilerError("OOX1000").WithLocation(0),
                    },
                },
            }.RunAsync();
        }

        [Fact]
        public async Task ConcreteOnly()
        {
            var test = @"using DocumentFormat.OpenXml;

namespace Test;

public class TestImpl
{
}

public partial class Factory
{
    [KnownFeature(typeof(TestImpl))]
    private partial T Get<T>();
}";
            var created = @"// <auto-generated/>

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

namespace Test;

public partial class Factory
{
    private global::Test.TestImpl? _TestImpl;

    private partial T? Get<T>()
    {
        if (typeof(T) == typeof(global::Test.TestImpl))
        {
            if (_TestImpl is null)
            {
                _TestImpl = new global::Test.TestImpl();
            }

            return (T)(object)_TestImpl;
        }

        return default;
    }
}
";

            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { test },
                    GeneratedSources =
                    {
                        (typeof(KnownFeaturesGenerator), "KnownFeatureAttribute.cs", Attribute),
                        (typeof(KnownFeaturesGenerator), "Factory_Get.cs", created),
                    },
                },
            }.RunAsync();
        }

        [Fact]
        public async Task ConcreteOnlyThreadSafe()
        {
            var test = @"using DocumentFormat.OpenXml;

namespace Test;

public class TestImpl
{
}

public partial class Factory
{
    [KnownFeature(typeof(TestImpl))]
    [ThreadSafe]
    private partial T Get<T>();
}";
            var created = @"// <auto-generated/>

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

namespace Test;

public partial class Factory
{
    private readonly object _LockGet = new();

    private global::Test.TestImpl? _TestImpl;

    private partial T? Get<T>()
    {
        if (typeof(T) == typeof(global::Test.TestImpl))
        {
            if (_TestImpl is null)
            {
                lock(_LockGet)
                {
                    if (_TestImpl is null)
                    {
                        _TestImpl = new global::Test.TestImpl();
                    }
                }
            }

            return (T)(object)_TestImpl;
        }

        return default;
    }
}
";

            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { test },
                    GeneratedSources =
                    {
                        (typeof(KnownFeaturesGenerator), "KnownFeatureAttribute.cs", Attribute),
                        (typeof(KnownFeaturesGenerator), "Factory_Get.cs", created),
                    },
                },
            }.RunAsync();
        }

        [Fact]
        public async Task SingleGeneratorNestedClass()
        {
            var test = @"using DocumentFormat.OpenXml;

namespace Test;

public interface ITest
{
}

public class TestImpl : ITest
{
}

public partial class Factory
{
    private partial class Other
    {
        [KnownFeature(typeof(ITest), typeof(TestImpl))]
        [ThreadSafe]
        private partial T Get<T>();
    }
}";
            var created = @"// <auto-generated/>

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

namespace Test;

public partial class Factory
{
    private partial class Other
    {
        private readonly object _LockGet = new();

        private global::Test.ITest? _TestImpl;

        private partial T? Get<T>()
        {
            if (typeof(T) == typeof(global::Test.ITest))
            {
                if (_TestImpl is null)
                {
                    lock(_LockGet)
                    {
                        if (_TestImpl is null)
                        {
                            _TestImpl = new global::Test.TestImpl();
                        }
                    }
                }

                return (T)_TestImpl;
            }

            return default;
        }
    }
}
";

            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { test },
                    GeneratedSources =
                    {
                        (typeof(KnownFeaturesGenerator), "KnownFeatureAttribute.cs", Attribute),
                        (typeof(KnownFeaturesGenerator), "Other_Get.cs", created),
                    },
                },
            }.RunAsync();
        }

        [Fact]
        public async Task SingleGenerator()
        {
            var test = @"using DocumentFormat.OpenXml;

namespace Test;

public interface ITest
{
}

public class TestImpl : ITest
{
}

public partial class Factory
{
    [KnownFeature(typeof(ITest), typeof(TestImpl))]
    [ThreadSafe]
    private partial T Get<T>();
}";
            var created = @"// <auto-generated/>

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

namespace Test;

public partial class Factory
{
    private readonly object _LockGet = new();

    private global::Test.ITest? _TestImpl;

    private partial T? Get<T>()
    {
        if (typeof(T) == typeof(global::Test.ITest))
        {
            if (_TestImpl is null)
            {
                lock(_LockGet)
                {
                    if (_TestImpl is null)
                    {
                        _TestImpl = new global::Test.TestImpl();
                    }
                }
            }

            return (T)_TestImpl;
        }

        return default;
    }
}
";

            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { test },
                    GeneratedSources =
                    {
                        (typeof(KnownFeaturesGenerator), "KnownFeatureAttribute.cs", Attribute),
                        (typeof(KnownFeaturesGenerator), "Factory_Get.cs", created),
                    },
                },
            }.RunAsync();
        }

        [Fact]
        public async Task SingleGeneratorMultipleServices()
        {
            var test = @"using DocumentFormat.OpenXml;

namespace Test;

public interface ITest1
{
}

public class TestImpl1 : ITest1
{
}

public interface ITest2
{
}

public class TestImpl2 : ITest2
{
}

public partial class Factory
{
    [KnownFeature(typeof(ITest1), typeof(TestImpl1))]
    [KnownFeature(typeof(ITest2), typeof(TestImpl2))]
    [ThreadSafe]
    private partial T Get<T>();
}";
            var created = @"// <auto-generated/>

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

namespace Test;

public partial class Factory
{
    private readonly object _LockGet = new();

    private global::Test.ITest1? _TestImpl1;
    private global::Test.ITest2? _TestImpl2;

    private partial T? Get<T>()
    {
        if (typeof(T) == typeof(global::Test.ITest1))
        {
            if (_TestImpl1 is null)
            {
                lock(_LockGet)
                {
                    if (_TestImpl1 is null)
                    {
                        _TestImpl1 = new global::Test.TestImpl1();
                    }
                }
            }

            return (T)_TestImpl1;
        }

        if (typeof(T) == typeof(global::Test.ITest2))
        {
            if (_TestImpl2 is null)
            {
                lock(_LockGet)
                {
                    if (_TestImpl2 is null)
                    {
                        _TestImpl2 = new global::Test.TestImpl2();
                    }
                }
            }

            return (T)_TestImpl2;
        }

        return default;
    }
}
";

            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { test },
                    GeneratedSources =
                    {
                        (typeof(KnownFeaturesGenerator), "KnownFeatureAttribute.cs", Attribute),
                        (typeof(KnownFeaturesGenerator), "Factory_Get.cs", created),
                    },
                },
            }.RunAsync();
        }

        [Fact]
        public async Task SingleGeneratorMultipleServicesNotThreadSafe()
        {
            var test = @"using DocumentFormat.OpenXml;

namespace Test;

public interface ITest1
{
}

public class TestImpl1 : ITest1
{
}

public interface ITest2
{
}

public class TestImpl2 : ITest2
{
}

public partial class Factory
{
    [KnownFeature(typeof(ITest1), typeof(TestImpl1))]
    [KnownFeature(typeof(ITest2), typeof(TestImpl2))]
    private partial T Get<T>();
}";
            var created = @"// <auto-generated/>

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable enable

namespace Test;

public partial class Factory
{
    private global::Test.ITest1? _TestImpl1;
    private global::Test.ITest2? _TestImpl2;

    private partial T? Get<T>()
    {
        if (typeof(T) == typeof(global::Test.ITest1))
        {
            if (_TestImpl1 is null)
            {
                _TestImpl1 = new global::Test.TestImpl1();
            }

            return (T)_TestImpl1;
        }

        if (typeof(T) == typeof(global::Test.ITest2))
        {
            if (_TestImpl2 is null)
            {
                _TestImpl2 = new global::Test.TestImpl2();
            }

            return (T)_TestImpl2;
        }

        return default;
    }
}
";

            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { test },
                    GeneratedSources =
                    {
                        (typeof(KnownFeaturesGenerator), "KnownFeatureAttribute.cs", Attribute),
                        (typeof(KnownFeaturesGenerator), "Factory_Get.cs", created),
                    },
                },
            }.RunAsync();
        }
    }
}
