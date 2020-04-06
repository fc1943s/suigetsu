namespace Suigetsu.UI.Frontend.ElmishBridge

module Util =
    let jsArray<'T> =
        Array.toList<'T>
        >> List.toArray
