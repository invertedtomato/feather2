using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InvertedTomato.IO.Feather {
    public interface IMessage {
        void Load();
        Stream ToStream();
    }
}
