function inpoly(lat, lng, ring) {
    var inside = false;
    for (var i = 0, j = ring.length - 1; i < ring.length; j = i++) {
        var xi = ring[i][0], yi = ring[i][1];
        var xj = ring[j][0], yj = ring[j][1];

        var intersect = ((yi > lat) != (yj > lat)) &&
            (lng < (xj - xi) * (lat - yi) / (yj - yi) + xi);
        if (intersect) inside = !inside;
    }
    return inside;
}

function pointInPolygon(point, geometry, bounds) {
    var inbox = !(point.coordinates[0] > bounds._northEast.lng ||
        point.coordinates[1] > bounds._northEast.lat ||
        point.coordinates[0] < bounds._southWest.lng ||
        point.coordinates[1] < bounds._southWest.lat);
    if (!inbox)
        return false;

    var type = geometry.type;
    var coordinates = geometry.coordinates;
    var lat = point.coordinates[1];
    var lng = point.coordinates[0];

    if (type === 'Polygon') {
        return isPointInPolygonArray(lat, lng, coordinates);
    } else if (type === 'MultiPolygon') {
        for (var i = 0; i < coordinates.length; i++) {
            if (isPointInPolygonArray(lat, lng, coordinates[i])) {
                return true;
            }
        }
        return false;
    } else {
        return false; // Unsupported geometry type
    }
}

function isPointInPolygonArray(lat, lng, polygonArray) {
    // polygonArray is an array of linear rings
    // The first ring is the outer boundary; subsequent rings are holes
    var inside = false;

    // Check if point is inside outer boundary
    if (inpoly(lat, lng, polygonArray[0])) {
        inside = true;

        // Check if point is inside any holes
        for (var i = 1; i < polygonArray.length; i++) {
            if (inpoly(lat, lng, polygonArray[i])) {
                // Point is inside a hole; thus, it's outside the polygon
                inside = false;
                break;
            }
        }
    }

    return inside;
}

function pointInLayer(pointCoords, layerGroup, returnOnFirstFound) {
    "use strict";
    var layersFound = [];
    layerGroup.eachLayer(function (layer) {
        if (returnOnFirstFound && layersFound.length) return;
        if (layer instanceof L.Polygon) {
            var geometry = layer.toGeoJSON().geometry;
            var bounds = layer.getBounds();
            var point = {
                type: "Point",
                coordinates: pointCoords
            };
            if (pointInPolygon(point, geometry, bounds)) {
                layersFound.push(layer);
            }
        }
    });
    return layersFound;
}
