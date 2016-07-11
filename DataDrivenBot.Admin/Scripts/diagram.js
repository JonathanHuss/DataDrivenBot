var myDiagram;

function init(nodes, links) {
    if (window.goSamples) goSamples();  // init for these samples -- you don't need to call this
    var GO = go.GraphObject.make;  // for conciseness in defining templates

    myDiagram =
      GO(go.Diagram, "myDiagramDiv",  // must name or refer to the DIV HTML element
        {
            grid: GO(go.Panel, "Grid",
                    GO(go.Shape, "LineH", { stroke: "lightgray", strokeWidth: 0.5 }),
                    GO(go.Shape, "LineH", { stroke: "gray", strokeWidth: 0.5, interval: 10 }),
                    GO(go.Shape, "LineV", { stroke: "lightgray", strokeWidth: 0.5 }),
                    GO(go.Shape, "LineV", { stroke: "gray", strokeWidth: 0.5, interval: 10 })
                  ),
            allowDrop: true,  // must be true to accept drops from the Palette
            "draggingTool.dragsLink": true,
            "draggingTool.isGridSnapEnabled": true,
            "linkingTool.isUnconnectedLinkValid": true,
            "linkingTool.portGravity": 20,
            "relinkingTool.isUnconnectedLinkValid": true,
            "relinkingTool.portGravity": 20,
            "relinkingTool.fromHandleArchetype":
              GO(go.Shape, "Diamond", { segmentIndex: 0, cursor: "pointer", desiredSize: new go.Size(8, 8), fill: "tomato", stroke: "darkred" }),
            "relinkingTool.toHandleArchetype":
              GO(go.Shape, "Diamond", { segmentIndex: -1, cursor: "pointer", desiredSize: new go.Size(8, 8), fill: "darkred", stroke: "tomato" }),
            "linkReshapingTool.handleArchetype":
              GO(go.Shape, "Diamond", { desiredSize: new go.Size(7, 7), fill: "lightblue", stroke: "deepskyblue" }),
            //rotatingTool: GO(TopRotatingTool),  // defined below
            //"rotatingTool.snapAngleMultiple": 15,
            //"rotatingTool.snapAngleEpsilon": 15,
            "undoManager.isEnabled": true,
            layout:
                GO(go.TreeLayout,
                {
                    angle: 90
                }),
            initialContentAlignment: go.Spot.Center,

        });

    // when the document is modified, add a "*" to the title and enable the "Save" button
    myDiagram.addDiagramListener("Modified", function(e) {
        var button = document.getElementById("SaveButton");
        if (button) button.disabled = !myDiagram.isModified;
        var idx = document.title.indexOf("*");
        if (myDiagram.isModified) {
            if (idx < 0) document.title += "*";
        } else {
            if (idx >= 0) document.title = document.title.substr(0, idx);
        }
    });

    function makePorts(go) {
        makePort("T", go.Spot.Top, false, true);
        
        if (go.name == "Condition") {
            makePort("L", go.Spot.Left, true, true);
            makePort("R", go.Spot.Right, true, true);
        }
        else
            makePort("B", go.Spot.Bottom, true, false);
    }


    // Define a function for creating a "port" that is normally transparent.
    // The "name" is used as the GraphObject.portId, the "spot" is used to control how links connect
    // and where the port is positioned on the node, and the boolean "output" and "input" arguments
    // control whether the user can draw links from or to the port.
    function makePort(name, spot, output, input) {
        // the port is basically just a small transparent square
        return GO(go.Shape, "Circle",
                 {
                     fill: null,  // not seen, by default; set to a translucent gray by showSmallPorts, defined below
                     stroke: null,
                     desiredSize: new go.Size(7, 7),
                     alignment: spot,  // align the port on the main Shape
                     alignmentFocus: spot,  // just inside the Shape
                     portId: name,  // declare this object to be a "port"
                     fromSpot: spot, toSpot: spot,  // declare where links may connect at this port
                     fromLinkable: output, toLinkable: input,  // declare whether the user may draw links to/from here
                     cursor: "pointer"  // show a different cursor to indicate potential link point
                 },
                 new go.Binding("visible", "type", function (t) {
                     if ((t == "Condition" && (name == "T" || name == "R" || name == "L")) ||
                        (t != "Condition" && (name == "T" || name == "B")))
                         return true;
                     else
                         return false;
                 }));
            
    }

    var nodeSelectionAdornmentTemplate =
      GO(go.Adornment, "Auto",
        GO(go.Shape, { fill: null, stroke: "deepskyblue", strokeWidth: 1.5, strokeDashArray: [4, 2] }),
        GO(go.Placeholder)
      );

    var nodeResizeAdornmentTemplate =
      GO(go.Adornment, "Spot",
        { locationSpot: go.Spot.Right },
        GO(go.Placeholder),
        GO(go.Shape, { alignment: go.Spot.TopLeft, cursor: "nw-resize", desiredSize: new go.Size(6, 6), fill: "lightblue", stroke: "deepskyblue" }),
        GO(go.Shape, { alignment: go.Spot.Top, cursor: "n-resize", desiredSize: new go.Size(6, 6), fill: "lightblue", stroke: "deepskyblue" }),
        GO(go.Shape, { alignment: go.Spot.TopRight, cursor: "ne-resize", desiredSize: new go.Size(6, 6), fill: "lightblue", stroke: "deepskyblue" }),

        GO(go.Shape, { alignment: go.Spot.Left, cursor: "w-resize", desiredSize: new go.Size(6, 6), fill: "lightblue", stroke: "deepskyblue" }),
        GO(go.Shape, { alignment: go.Spot.Right, cursor: "e-resize", desiredSize: new go.Size(6, 6), fill: "lightblue", stroke: "deepskyblue" }),

        GO(go.Shape, { alignment: go.Spot.BottomLeft, cursor: "se-resize", desiredSize: new go.Size(6, 6), fill: "lightblue", stroke: "deepskyblue" }),
        GO(go.Shape, { alignment: go.Spot.Bottom, cursor: "s-resize", desiredSize: new go.Size(6, 6), fill: "lightblue", stroke: "deepskyblue" }),
        GO(go.Shape, { alignment: go.Spot.BottomRight, cursor: "sw-resize", desiredSize: new go.Size(6, 6), fill: "lightblue", stroke: "deepskyblue" })
      );

    //var nodeRotateAdornmentTemplate =
    //  GO(go.Adornment,
    //    { locationSpot: go.Spot.Center, locationObjectName: "CIRCLE" },
    //    GO(go.Shape, "Circle", { name: "CIRCLE", cursor: "pointer", desiredSize: new go.Size(7, 7), fill: "lightblue", stroke: "deepskyblue" }),
    //    GO(go.Shape, { geometryString: "M3.5 7 L3.5 30", isGeometryPositioned: true, stroke: "deepskyblue", strokeWidth: 1.5, strokeDashArray: [4, 2] })
    //  );

    myDiagram.nodeTemplate =
      GO(go.Node, "Spot",
        { locationSpot: go.Spot.Center },
        new go.Binding("name", "type"),
        new go.Binding("location", "loc", go.Point.parse).makeTwoWay(go.Point.stringify),
        { selectable: true, selectionAdornmentTemplate: nodeSelectionAdornmentTemplate },
        { resizable: true, resizeObjectName: "PANEL", resizeAdornmentTemplate: nodeResizeAdornmentTemplate },
        //{ rotatable: true, rotateAdornmentTemplate: nodeRotateAdornmentTemplate },
        new go.Binding("angle").makeTwoWay(),
        // the main object is a Panel that surrounds a TextBlock with a Shape
        GO(go.Panel, "Auto",
          { name: "PANEL" },
          new go.Binding("desiredSize", "size", go.Size.parse).makeTwoWay(go.Size.stringify),
          GO(go.Shape, "RoundedRectangle",  // default figure
            {
                portId: "", // the default port: if no spot on link data, use closest side
                fromLinkable: true, toLinkable: true, cursor: "pointer",
                fill: "white",  // default color
                strokeWidth: 2
            },
            new go.Binding("figure", "type", function (t) {
                if (t == "Condition")
                    return "Diamond";
                else
                    return "RoundedRectange";
            }),
            new go.Binding("fill", "type", function (t) {
                switch (t) {
                    case "Question":
                        return "lightgreen";
                    case "ServiceCall":
                        return "lightskyblue";
                    case "Display":
                        return "lightyellow";
                    case "Condition":
                        return "orange";
                }
            }),
            new go.Binding("itemArray", "actions")),
          GO(go.TextBlock,
            {
                font: "bold 11pt Helvetica, Arial, sans-serif",
                margin: 8,
                maxSize: new go.Size(160, NaN),
                wrap: go.TextBlock.WrapFit,
                editable: true
            },
            new go.Binding("text", "title").makeTwoWay())
        ),
        // four small named ports, one on each side:
        makePort("T", go.Spot.Top, false, true),
        makePort("L", go.Spot.Left, true, true),
        makePort("R", go.Spot.Right, true, true),
        makePort("B", go.Spot.Bottom, true, false),
        { // handle mouse enter/leave events to show/hide the ports
            mouseEnter: function(e, node) { showSmallPorts(node, true); },
            mouseLeave: function(e, node) { showSmallPorts(node, false); }
        }
      );

    function showSmallPorts(node, show) {
        node.ports.each(function(port) {
            if (port.portId !== "") {  // don't change the default port, which is the big shape
                port.fill = show ? "rgba(0,0,0,.3)" : null;
            }
        });
    }

    var linkSelectionAdornmentTemplate =
      GO(go.Adornment, "Link",
        GO(go.Shape,
          // isPanelMain declares that this Shape shares the Link.geometry
          { isPanelMain: true, fill: null, stroke: "deepskyblue", strokeWidth: 0 })  // use selection object's strokeWidth
      );

    myDiagram.linkTemplate =
      GO(go.Link,  // the whole link panel
        { selectable: true, selectionAdornmentTemplate: linkSelectionAdornmentTemplate },
        { relinkableFrom: true, relinkableTo: true, reshapable: true },
        {
            routing: go.Link.AvoidsNodes,
            curve: go.Link.JumpOver,
            corner: 5,
            toShortLength: 4
        },
        new go.Binding("points").makeTwoWay(),
        GO(go.Shape,  // the link path shape
          { isPanelMain: true, strokeWidth: 2 }),
        GO(go.Shape,  // the arrowhead
          { toArrow: "Standard", stroke: null }),
        GO(go.Panel, "Auto",
          //new go.Binding("visible", "isSelected").ofObject(),
          GO(go.Shape, "RoundedRectangle",  // the link shape
            { fill: "#F8F8F8", stroke: null }),
          GO(go.TextBlock,
            {
                textAlign: "center",
                font: "10pt helvetica, arial, sans-serif",
                stroke: "black",
                margin: 2,
                minSize: new go.Size(10, NaN),
                editable: true
            },
            new go.Binding("text", "answer").makeTwoWay())
        )
      );

    //load();  // load an initial diagram from some JSON text

    // initialize the Palette that is on the left side of the page
    myPalette =
      GO(go.Palette, "myPaletteDiv",  // must name or refer to the DIV HTML element
        {
            maxSelectionCount: 1,
            nodeTemplateMap: myDiagram.nodeTemplateMap,  // share the templates used by myDiagram
            linkTemplate: // simplify the link template, just in this Palette
              GO(go.Link,
                { // because the GridLayout.alignment is Location and the nodes have locationSpot == Spot.Center,
                    // to line up the Link in the same manner we have to pretend the Link has the same location spot
                    locationSpot: go.Spot.Center,
                    selectionAdornmentTemplate:
                      GO(go.Adornment, "Link",
                        { locationSpot: go.Spot.Center },
                        GO(go.Shape,
                          { isPanelMain: true, fill: null, stroke: "deepskyblue", strokeWidth: 0 }),
                        GO(go.Shape,  // the arrowhead
                          { toArrow: "Standard", stroke: null })
                      )
                },
                {
                    routing: go.Link.AvoidsNodes,
                    curve: go.Link.JumpOver,
                    corner: 5,
                    toShortLength: 4
                },
                new go.Binding("points"),
                GO(go.Shape,  // the link path shape
                  { isPanelMain: true, strokeWidth: 2 }),
                GO(go.Shape,  // the arrowhead
                  { toArrow: "Standard", stroke: null })
              ),
            model: new go.GraphLinksModel([  // specify the contents of the Palette
              //{ title: "Start", figure: "Circle", fill: "#00AD5F" },
              { title: "Question", type: "Question", prompt: "", retryPrompt: "", startingStep: false },
              { title: "Service Call", type: "ServiceCall", serviceUrl: "", headers: "", method: "", startingStep: false },
              { title: "Display", type: "Display", text: "", startingStep: false },
              { title: "Condition", type: "Condition", text: "", startingStep: false }
              //{ title: "End", figure: "Circle", fill: "#CE0620" }
            ], [
              // the Palette also has a disconnected Link, which the user can drag-and-drop
              { points: new go.List(go.Point).addAll([new go.Point(0, 0), new go.Point(30, 0), new go.Point(30, 40), new go.Point(60, 40)]) }
            ])
        });

    myDiagram.model =
      GO(go.GraphLinksModel,
        {
            linkFromPortIdProperty: "fromPort",  
            linkToPortIdProperty: "toPort",
            nodeDataArray: nodes,
            linkDataArray: links
        });

    $(function () {
        //GO("#paletteDraggable").draggable({ handle: "#paletteDraggableHandle" }).resizable({
        //    // After resizing, perform another layout to fit everything in the palette's viewport
        //    stop: function () { myPalette.layoutDiagram(true); }
        //});
        //$("#infoDraggable").draggable({ handle: "#infoDraggableHandle" });
        var inspector = new Inspector('myInfoDiv', myDiagram,
          {
              properties: {
                  // key would be automatically added for nodes, but we want to declare it read-only also:
                  "key": { readOnly: true, show: Inspector.showIfPresent },
                  // fill and stroke would be automatically added for nodes, but we want to declare it a color also:
                  //"fill": { show: Inspector.showIfPresent, type: 'color' },
                  //"stroke": { show: Inspector.showIfPresent, type: 'color' }
              }
          });
    });

}

    // create the Model with the above data, and assign to the Diagram

    //myDiagram.model =
    //      GO(go.GraphLinksModel,
    //        {
    //            nodeDataArray: nodeDataArray,
    //            linkDataArray: linkDataArray
    //        });



    // properties declare which properties to show and how.
    // By default, all properties on the model data objects are shown unless the inspector option "includesOwnProperties" is set to false.

    // Show the primary selection's data, or blanks if no Part is selected:
    //var inspector = new Inspector('myInspectorDiv', myDiagram,
    //  {
    //      // uncomment this line to only inspect the named properties below instead of all properties on each object:
    //      includesOwnProperties: false,
    //      properties: {
    //          "title": {},
    //          "prompt": {},
    //          "retryPrompt": {}
              
    //      }
    //  });


    


function Save() {
    var json = myDiagram.model.toJSON();

    $("#jsonDiagram").val(json);

    $("#SaveForm").submit();
        
}
