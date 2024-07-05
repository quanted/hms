function inpoly(n, t, o) {
    var e = [[0, 0]]; 
        for (var a = 0; a < o.length; a++)
            e.push(o[a]);
        e.push(o[0]),
        e.push([0, 0])
                    
    for (var i = !1, r = 0, a = e.length - 1; r < e.length; a = r++)
        e[r][0] > t != e[a][0] > t && n < (e[a][1] - e[r][1]) * (t - e[r][0]) / (e[a][0] - e[r][0]) + e[r][1] && (i = !i);
    return i
}

pointInPolygon = function (t, r, b) {
    inbox = !(t.coordinates[0] > b._northEast.lng || t.coordinates[1] > b._northEast.lat || t.coordinates[0] < b._southWest.lng || t.coordinates[1] < b._southWest.lat)
    if (!inbox)
        return !1;
    for (var u = !1, s = 0; s < r.coordinates.length; s++)
        inpoly(t.coordinates[1], t.coordinates[0], r.coordinates[s]) && (u = !0);
    return u
}

pointInLayer= function(n, t, r) {
    "use strict";
    var a = [];
    return t.eachLayer(function (t) {
        r && a.length || (t instanceof L.Polygon) && pointInPolygon({
            type: "Point",
            coordinates: n
        }, t.toGeoJSON().geometry, t.getBounds()) && a.push(t)
    }),
    a
}

