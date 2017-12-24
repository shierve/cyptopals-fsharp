﻿open System.IO
open Crypto.Data
open Crypto.Analysis
open Crypto.Encryption

let ch1 () =
    let ch1String = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";
    let d = Data.fromHex ch1String
    Data.asB64 d |> printfn "%A"

let ch2 () =
    let d1 = Data.fromHex "1c0111001f010100061a024b53535009181c"
    let d2 = Data.fromHex "686974207468652062756c6c277320657965"
    let xord = d1 ^^^ d2
    Data.asHex xord |> printfn "%A"

let ch3 () =
    let cipher = Data.fromHex "1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736"
    let candidates: (byte * Data)[] = List.map ( fun k -> (byte k, (cipher ^^ (byte k))) ) [0 .. 255] |> List.toArray
    let sortedCandidates = frequencySort candidates
    printfn "There are %A valid candidates:" (Array.length sortedCandidates)
    Array.iter (fun (k, d, s) -> printfn "[key: %A] %A (Score: %A)" k (Data.asString d) s) sortedCandidates

let ch4 () =
    let path = __SOURCE_DIRECTORY__ + "/data/ch4.txt"
    let bestKeys =
        File.ReadAllLines path
        |> Array.map (Data.fromHex >> bestKey)
        |> Array.mapi
            (fun i opt ->
                match opt with
                | Some (k, d, s) -> (i, k, d, s)
                | None -> (i, 0uy, Data([||]), -1.)
            )
        |> Array.filter (fun (_, _, _, s) -> s > 0.)
        |> Array.sortBy (fun (_, _, _, s) -> s)
    Array.iter (fun (i, k, d, s) -> printfn "[line: %A][key: %A][score: %A] %A" i k s (Data.asString d)) bestKeys

let ch5 () =
    let str = """Burning 'em, if you ain't quick and nimble
I go crazy when I hear a cymbal"""
    repeatingKeyXor (Data.fromString str) "ICE"
    |> (Data.asHex >> printfn "%A")

let ch6 () =
    printfn "Edit distance: %A" (hammingDistance (Data.fromString "this is a test") (Data.fromString "wokka wokka!!!"))

[<EntryPoint>]
let main argv =
    ch6 () |> ignore
    0 // return an integer exit code