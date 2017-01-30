#region license
// Copyright © 2016, 2017 Maxim Trushin (dba Syntactik, trushin@gmail.com, maxim.trushin@syntactik.com)
//
// This file is part of Malina.
// Malina is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Malina is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Malina.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using System.Collections.Generic;
using NUnit.Framework;


namespace mlc.Tests
{
    [TestFixture, Category("ArgumentParser")]
    public class ArgumentParserTestFixture
    {
        [Test]
        public void NoFilesSpecified()
        {
            List<string> files;
            bool recursive;
            string outputDirectory;
            Assert.Throws<ArgumentsParserException>(
                () => ArgumentsParser.Parse(new string[] {}, out files, out recursive, out outputDirectory),
                ArgumentsParser.ParameterErrors.NoInput);
        }
    }
}