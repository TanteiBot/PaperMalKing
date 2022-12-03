﻿#region LICENSE
// PaperMalKing.
// Copyright (C) 2021-2022 N0D4N
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaperMalKing.UpdatesProviders.Base.Features
{
    // ReSharper disable once TypeParameterCanBeVariant
    public interface IUserFeaturesService<T> where T : unmanaged, Enum, IComparable, IConvertible, IFormattable
    {
        IReadOnlyDictionary<T, (string,string)> Descriptions { get; }
        Task EnableFeaturesAsync(IReadOnlyList<T> features, ulong userId);

        Task DisableFeaturesAsync(IReadOnlyList<T> features, ulong userId);

        ValueTask<string> EnabledFeaturesAsync(ulong userId);
    }
}