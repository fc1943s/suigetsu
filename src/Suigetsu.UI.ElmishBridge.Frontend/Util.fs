namespace Suigetsu.UI.ElmishBridge.Frontend

module Util =
    let jsArray<'T> =
        Array.toList<'T>
        >> List.toArray
