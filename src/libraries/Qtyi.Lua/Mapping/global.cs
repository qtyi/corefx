// Licensed to the Qtyi under one or more agreements.
// The Qtyi licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Qtyi.Runtime.CompilerServices;
using Object = Qtyi.Runtime.Object;

[assembly: LuaField("pairs", UnderlyingType = typeof(Object), MemberName = nameof(Object.Pairs))]
