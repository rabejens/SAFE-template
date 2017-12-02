module App

open Elmish
open Elmish.React

open Fable.Helpers.React.Props
module R = Fable.Helpers.React
open Fable.PowerPack.Fetch

#if (Fulma)
open Fulma
open Fulma.Layouts
open Fulma.Elements
open Fulma.Components
open Fulma.BulmaClasses
#endif

type Model = int option

type Msg = Increment | Decrement | Init of Result<int, exn>

let init () = 
  let model = None
  let cmd = 
    Cmd.ofPromise 
      (fetchAs<int> "/api/init") 
      [] 
      (Ok >> Init) 
      (Error >> Init)
  model, cmd

let update msg (model : Model) =
  let model' =
    match model,  msg with
    | Some x, Increment -> Some (x + 1)
    | Some x, Decrement -> Some (x - 1)
    | None, Init (Ok x) -> Some x
    | _ -> None
  model', Cmd.none

let safeComponents =
  let intersperse sep ls =
    List.foldBack (fun x -> function
      | [] -> [x]
      | xs -> x::sep::xs) ls []

  let components =
    [ 
      "Suave.IO", "http://suave.io" 
      "Fable"   , "http://fable.io"
      "Elmish"  , "https://fable-elmish.github.io/"
#if (Fulma)
      "Fulma"   , "https://mangelmaxime.github.io/Fulma" 
#endif
    ]
    |> List.map (fun (desc,link) -> R.a [ Href link ] [ R.str desc ] )
    |> intersperse (R.str ", ")
    |> R.span [ ]

  R.p [ ]
    [ R.strong [] [ R.str "SAFE Template" ]
      R.str " powered by: "
      components ]

let show = function
| Some x -> string x
| None -> "Loading..."

#if (Fulma)
let button txt onClick = 
  Button.button_btn
    [ Button.isFullWidth
      Button.isPrimary
      Button.onClick onClick ] 
    [ R.str txt ]
#endif

let view model dispatch =
#if (Fulma)
  R.div []
    [ Navbar.navbar [ Navbar.customClass "is-primary" ]
        [ Navbar.item_div [ ]
            [ Heading.h2 [ ]
                [ R.str "SAFE Template" ] ] ]

      Container.container []
        [ Content.content [ Content.customClass Bulma.Level.Item.HasTextCentered ] 
            [ Heading.h3 [] [ R.str ("Press buttons to manipulate counter: " + show model) ] ]
          Columns.columns [] 
            [ Column.column [] [ button "-" (fun _ -> dispatch Decrement) ]
              Column.column [] [ button "+" (fun _ -> dispatch Increment) ] ] ]
    
      Footer.footer [ ]
        [ Content.content [ Content.customClass Bulma.Level.Item.HasTextCentered ]
            [ safeComponents ] ] ]
#else
  R.div []
    [ R.h1 [] [ R.str "SAFE Template" ]
      R.p  [] [ R.str "The initial counter is fetched from server" ]
      R.p  [] [ R.str "Press buttons to manipulate counter:" ]
      R.button [ OnClick (fun _ -> dispatch Decrement) ] [ R.str "-" ]
      R.div [] [ R.str (show model) ]
      R.button [ OnClick (fun _ -> dispatch Increment) ] [ R.str "+" ]
      safeComponents ]
#endif
  
//-:cnd:noEmit
#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
//+:cnd:noEmit
|> Program.run
