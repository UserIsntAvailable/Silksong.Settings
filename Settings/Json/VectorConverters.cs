// Copyright notice from hk-modding/api:
//
// MIT License
//
// Copyright (c) 2017 seanpr96, iamwyza, firzen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Settings;

/// <inheritdoc />
public class Vector2Converter : JsonConverter<Vector2>
{
    /// <inheritdoc />
    public override Vector2 ReadJson(Dictionary<string, object> token, object existingValue)
    {
        float x = Convert.ToSingle(token["x"]);
        float y = Convert.ToSingle(token["y"]);
        return new Vector2(x, y);
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, Vector2 value)
    {
        writer.WritePropertyName("x");
        writer.WriteValue(value.x);
        writer.WritePropertyName("y");
        writer.WriteValue(value.y);
    }
}

/// <inheritdoc />
public class Vector3Converter : JsonConverter<Vector3>
{
    /// <inheritdoc />
    public override Vector3 ReadJson(Dictionary<string, object> token, object existingValue)
    {
        float x = Convert.ToSingle(token["x"]);
        float y = Convert.ToSingle(token["y"]);
        float z = Convert.ToSingle(token["z"]);
        return new Vector3(x, y, z);
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, Vector3 value)
    {
        writer.WritePropertyName("x");
        writer.WriteValue(value.x);
        writer.WritePropertyName("y");
        writer.WriteValue(value.y);
        writer.WritePropertyName("z");
        writer.WriteValue(value.z);
    }
}
