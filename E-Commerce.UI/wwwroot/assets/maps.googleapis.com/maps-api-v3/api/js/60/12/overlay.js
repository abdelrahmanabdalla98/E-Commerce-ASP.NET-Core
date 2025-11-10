google.maps.__gjsload__('overlay', function(_) {
    var iCa = function() {},
        mE = function(a) {
            a.hB = a.hB || new iCa;
            return a.hB
        },
        jCa = function(a) {
            this.Eg = new _.xm(() => {
                const b = a.hB;
                if (a.getPanes()) {
                    if (a.getProjection()) {
                        if (!b.Gg && a.onAdd) a.onAdd();
                        b.Gg = !0;
                        a.draw()
                    }
                } else {
                    if (b.Gg)
                        if (a.onRemove) a.onRemove();
                        else a.remove();
                    b.Gg = !1
                }
            }, 0)
        },
        lCa = function(a, b) {
            const c = mE(a);
            let d = c.Fg;
            d || (d = c.Fg = new jCa(a));
            _.xb(c.Eg || [], _.Xj);
            var e = c.Ig = c.Ig || new _.vta;
            const f = b.__gm;
            e.bindTo("zoom", f);
            e.bindTo("offset", f);
            e.bindTo("center", f, "projectionCenterQ");
            e.bindTo("projection",
                b);
            e.bindTo("projectionTopLeft", f);
            e = c.Kg = c.Kg || new kCa(e);
            e.bindTo("zoom", f);
            e.bindTo("offset", f);
            e.bindTo("projection", b);
            e.bindTo("projectionTopLeft", f);
            a.bindTo("projection", e, "outProjection");
            a.bindTo("panes", f);
            e = () => _.ym(d.Eg);
            c.Eg = [_.Vj(a, "panes_changed", e), _.Vj(f, "zoom_changed", e), _.Vj(f, "offset_changed", e), _.Vj(b, "projection_changed", e), _.Vj(f, "projectioncenterq_changed", e)];
            _.ym(d.Eg);
            b instanceof _.Ck ? (_.Sk(b, "Ox"), _.Q(b, 148440)) : b instanceof _.jl && (_.Sk(b, "Oxs"), _.Q(b, 181451))
        },
        mCa = function(a) {
            const b =
                mE(a);
            var c = b.Ig;
            c && c.unbindAll();
            (c = b.Kg) && c.unbindAll();
            a.unbindAll();
            a.set("panes", null);
            a.set("projection", null);
            b.Eg && b.Eg.forEach(d => {
                _.Xj(d)
            });
            b.Eg = null;
            b.Fg && (_.zm(b.Fg.Eg), b.Fg = null)
        },
        rCa = function(a) {
            if (a) {
                var b = a.getMap();
                if (nCa(a) !== b && b && b instanceof _.Ck) {
                    const c = b.__gm;
                    c.overlayLayer ? a.__gmop = new oCa(b, a, c.overlayLayer) : c.Fg.then(({
                        ah: d
                    }) => {
                        const e = new pCa(b, d);
                        d.Li(e);
                        c.overlayLayer = e;
                        qCa(a);
                        rCa(a)
                    })
                }
            }
        },
        qCa = function(a) {
            if (a) {
                var b = a.__gmop;
                b && (a.__gmop = null, b.overlay.unbindAll(), b.overlay.set("panes",
                    null), b.overlay.set("projection", null), b.overlayLayer.Un(b), b.Eg && (b.Eg = !1, b.overlay.onRemove ? b.overlay.onRemove() : b.overlay.remove()))
            }
        },
        nCa = function(a) {
            return (a = a.__gmop) ? a.map : null
        },
        sCa = function(a, b) {
            a.overlay.get("projection") !== b && (a.overlay.bindTo("panes", a.map.__gm), a.overlay.set("projection", b))
        },
        kCa = class extends _.kk {
            constructor(a) {
                super();
                this.projection = a
            }
            changed(a) {
                a !== "outProjection" && (a = !!(this.get("offset") && this.get("projectionTopLeft") && this.get("projection") && _.$i(this.get("zoom"))),
                    a === !this.get("outProjection") && this.set("outProjection", a ? this.projection : null))
            }
        };
    _.Ca(jCa, _.kk);
    var oCa = class {
            constructor(a, b, c) {
                this.map = a;
                this.overlay = b;
                this.overlayLayer = c;
                this.Eg = !1;
                _.Sk(this.map, "Ox");
                _.Q(this.map, 148440);
                c.so(this)
            }
            draw() {
                this.Eg || (this.Eg = !0, this.overlay.onAdd && this.overlay.onAdd());
                this.overlay.draw && this.overlay.draw()
            }
        },
        pCa = class {
            constructor(a, b) {
                this.map = a;
                this.ah = b;
                this.Eg = null;
                this.Fg = []
            }
            dispose() {}
            Nh(a, b, c, d, e, f, g, h) {
                const l = this.Eg = this.Eg || new _.Lz(this.map, this.ah, () => {});
                l.Nh(a, b, c, d, e, f, g, h);
                for (const n of this.Fg) sCa(n, l), n.draw()
            }
            so(a) {
                this.Fg.push(a);
                this.Eg &&
                    sCa(a, this.Eg);
                this.ah.refresh()
            }
            Un(a) {
                _.Ib(this.Fg, a)
            }
        };
    _.Ki("overlay", {
        YC: function(a) {
            if (a) {
                mCa(a);
                delete mE(a).Jg;
                qCa(a);
                var b = a.getMap();
                b && (b instanceof _.Ck ? rCa(a) : a && (b = a.getMap(), (mE(a).Jg || null) !== b && (b && lCa(a, b), mE(a).Jg = b)))
            }
        },
        preventMapHitsFrom: a => {
            _.Qv(a, {
                Ql: ({
                    event: b
                }) => {
                    _.Vt(b.Eg)
                },
                tk: b => _.Av(b),
                Fq: b => _.Bv(b),
                ul: b => _.Bv(b),
                Gk: b => _.Cv(b)
            }).Qq(!0)
        },
        preventMapHitsAndGesturesFrom: a => {
            a.addEventListener("click", _.Tj);
            a.addEventListener("contextmenu", _.Tj);
            a.addEventListener("dblclick", _.Tj);
            a.addEventListener("mousedown", _.Tj);
            a.addEventListener("mousemove",
                _.Tj);
            a.addEventListener("MSPointerDown", _.Tj);
            a.addEventListener("pointerdown", _.Tj);
            a.addEventListener("touchstart", _.Tj);
            a.addEventListener("wheel", _.Tj)
        }
    });
});