namespace Suigetsu.Core

open System
open System.Collections

module AlphaNum =
    let sort (a: string) (b: string) =
        let isnum (s: string) i =
            let c = s.Chars i
            c >= '0' && c <= '9'

        let chunk (s: string) f t =
            (f < s.Length) && (t < s.Length) && (isnum s f) = (isnum s t)

        let chunkto s f =
            let rec to_ s f e =
                if chunk s f e
                then to_ s f (e + 1)
                else e
            to_ s f f

        let int_of_string str =
            let success, v = Int32.TryParse str
            if success
            then v
            else 0

        let rec chunkcmp (a: string) ai (b: string) bi =
            let al, bl = a.Length, b.Length
            if ai >= al || bi >= bl then
                compare al bl
            else
                let ae, be = chunkto a ai, chunkto b bi
                let sa, sb = a.Substring (ai, (ae - ai)), b.Substring (bi, (be - bi))

                let cmp =
                    if isnum a ai && isnum b bi
                    then compare (int_of_string sa) (int_of_string sb)
                    else compare sa sb
                if cmp = 0
                then chunkcmp a ae b be
                else cmp

        chunkcmp a 0 b 0


    type AlphaNumComparer () =
        interface IComparer with
            member this.Compare (x, y) = sort (string x) (string y)
