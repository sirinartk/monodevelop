﻿namespace MonoDevelop.FSharp

open System
open ExtCore.Control
open Gtk
open MonoDevelop.Components.Docking
open MonoDevelop.Components
open MonoDevelop.Core
open MonoDevelop.DesignerSupport
open MonoDevelop.Ide.Editor.Extension
open MonoDevelop.Ide.Editor
open MonoDevelop.Ide.Gui.Components
open MonoDevelop.Ide.Gui.Content
open MonoDevelop.Ide.TypeSystem
open MonoDevelop.Ide
open MonoDevelop.Projects
open Microsoft.FSharp.Compiler.SourceCodeServices

type FSharpOutlineTextEditorExtension() = 
    inherit TextEditorExtension()
    let mutable treeView : PadTreeView option = None
    let mutable refreshingOutline : bool = false
    let mutable timerId : uint32 = 0u

    override x.Initialize() = 
        base.Initialize()
        x.DocumentContext.DocumentParsed.Add(x.UpdateDocumentOutline)
    
    override x.IsValidInContext _ = true
    
    //    IdeApp.Workbench.ActiveDocument <> null && IdeApp.Workbench.ActiveDocument.Name = x.DocumentContext.Name
    member private x.UpdateDocumentOutline _ = 
        if not refreshingOutline then
            refreshingOutline <- true
            timerId <- GLib.Timeout.Add (3000u, (fun _ -> x.refillTree))

    member private x.refillTree = 

        let ast = maybe { let! context = x.DocumentContext |> Option.ofNull
                          let! parsedDocument = context.ParsedDocument |> Option.ofNull
                          let! ast = parsedDocument.Ast |> Option.tryCast<ParseAndCheckResults>
                          return ast }

        DispatchService.AssertGuiThread()
        Gdk.Threads.Enter()
        ast |> Option.iter (fun ast -> 
                   match treeView with
                   | Some(treeView) -> 
                       if treeView.Model = null || not treeView.IsRealized then
                           false
                       let treeStore = treeView.Model :?> TreeStore
                       treeStore.Clear()
                       let toplevel = ast.GetNavigationItems()
                       for item in toplevel do
                           let iter = treeStore.AppendValues(item.Declaration)
                           for nested in item.Nested do
                               treeStore.AppendValues(iter, [| nested |]) |> ignore
                       treeView.ExpandAll()
                   | None -> ())
        Gdk.Threads.Leave()
        refreshingOutline <- false
        timerId <- 0u
        false
 
    override x.Dispose() = 
        // more stuff here
        printf "%s" "Disposed"
        base.Dispose()
    interface IOutlinedDocument with
        member x.GetOutlineWidget() = 
            match treeView with
            | Some(treeView) -> treeView :> Widget
            | None -> 
                let treeStore = new TreeStore(typedefof<obj>)
                let padTreeView = new PadTreeView(treeStore)

                let setCellIcon (_) (cellRenderer : CellRenderer) (treeModel : TreeModel) (iter : TreeIter) = 
                    let pixRenderer = cellRenderer :?> CellRendererImage
                    let item = treeModel.GetValue(iter, 0) :?> FSharpNavigationDeclarationItem
                    pixRenderer.Image <- ImageService.GetIcon(ServiceUtils.getIcon item.Glyph, Gtk.IconSize.Menu)
                
                let setCellText (_) (cellRenderer : CellRenderer) (treeModel : TreeModel) (iter : TreeIter) = 
                    let renderer = cellRenderer :?> CellRendererText
                    let item = treeModel.GetValue(iter, 0) :?> FSharpNavigationDeclarationItem
                    renderer.Text <- item.Name
                let jumpToDeclaration _ =
                    let iter : TreeIter ref = ref Unchecked.defaultof<_>
                    match padTreeView.Selection.GetSelected(iter) with
                    | true -> let node = padTreeView.Model.GetValue(!iter, 0) :?> FSharpNavigationDeclarationItem
                              x.Editor.CaretLine <- node.Range.StartLine
                              x.Editor.CaretColumn <- node.Range.StartColumn
                              ()
                    | false -> ()

                    () 
//                let refillTree _ = 
//                    DispatchService.AssertGuiThread()
//                    Gdk.Threads.Enter()
//                    treeStore.AppendValues("23") |> ignore
//                    Gdk.Threads.Leave()
                
                treeView <- Some padTreeView

                let pixRenderer = new CellRendererImage()
                pixRenderer.Xpad <- 0u
                pixRenderer.Ypad <- 0u
                padTreeView.TextRenderer.Xpad <- 0u
                padTreeView.TextRenderer.Ypad <- 0u

                let treeCol = new TreeViewColumn()
                treeCol.PackStart(pixRenderer, false)
                // treeCol.AddAttribute (padTreeView.TextRenderer,"text",1)
                treeCol.SetCellDataFunc(pixRenderer, new TreeCellDataFunc(setCellIcon))
                treeCol.PackStart(padTreeView.TextRenderer, true)
                treeCol.SetCellDataFunc(padTreeView.TextRenderer, new TreeCellDataFunc(setCellText))

                padTreeView.AppendColumn treeCol |> ignore
                padTreeView.HeadersVisible <- true
                padTreeView.Realized.Add(fun _ -> x.refillTree |> ignore)
                padTreeView.Selection.Changed.Subscribe(fun _ -> jumpToDeclaration false) |> ignore
                padTreeView.RowActivated.Subscribe(fun _ -> jumpToDeclaration true) |> ignore

                let sw = new CompactScrolledWindow()
                sw.Add padTreeView
                sw.ShowAll()
                sw :> Widget
        
        member x.GetToolbarWidgets() = List.empty<Widget> :> _
        member x.ReleaseOutlineWidget() = ()
