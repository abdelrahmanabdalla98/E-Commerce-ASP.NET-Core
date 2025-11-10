google.maps.__gjsload__('search', function(_) {
    var Gya = function(a) {
            const b = [];
            a.data.forEach(c => {
                b.push(...Fya(c))
            });
            return b
        },
        Fya = function(a) {
            let b = _.ml,
                c = -1;
            a.tiles.forEach(e => {
                e.zoom > c && (b = e.mi, c = e.zoom)
            });
            if (c === -1) return [];
            const d = [];
            a.Yu().forEach(e => {
                e.a && e.a.length >= 2 && d.push(new Hya(e, b, c))
            });
            return d
        },
        Iya = class extends _.kk {};
    var Jya = {
            ["1"]: {}
        },
        Hya = class {
            constructor(a, b, c) {
                this.sn = b;
                this.zoom = c;
                this.bounds = this.anchor = null;
                this.Eg = Jya;
                this.source = a;
                this.featureId = this.source.id || "0";
                this.infoWindowOffset = (this.source.io || []).length === 2 ? new google.maps.Point(this.source.io[0], this.source.io[1]) : null
            }
            getAnchor() {
                if (!this.anchor) {
                    const a = 1 << this.zoom;
                    this.anchor = _.xc(_.Nl(new _.gm((this.sn.x * 256 + this.source.a[0]) / a, (this.sn.y * 256 + this.source.a[1]) / a))).toJSON()
                }
                return this.anchor
            }
            getCompleteBounds() {
                return this.getBounds().reduce((a,
                    b) => {
                    a.extendByBounds(b);
                    return a
                }, _.Zl(0, 0, 0, 0))
            }
            getBounds() {
                if (this.bounds === null) {
                    this.bounds = [];
                    const a = this.source.bb || [];
                    if (a.length % 4 === 0)
                        for (let b = 0; b < a.length; b += 4) {
                            const c = this.bounds[this.bounds.length - 1],
                                d = _.Zl(a[b], a[b + 1], a[b + 2], a[b + 3]);
                            c && c.equals(d) || this.bounds.push(d)
                        }
                }
                return [...this.bounds]
            }
            getExtendedContent(a) {
                if (this.Eg === Jya) try {
                    this.Eg = this.source.c ? JSON.parse(this.source.c) : {}
                } catch (b) {
                    this.Eg = {}
                }
                return this.Eg[a] ? ? {}
            }
            getFeatureName() {
                return this.getExtendedContent("1") ? .title ? ?
                    null
            }
            isTransitStation() {
                return this.getExtendedContent("1") ? .is_transit_station ? ? !1
            }
        };
    var Kya = new WeakSet,
        FD = class extends Iya {
            constructor(a) {
                super();
                this.setValues(a);
                this.setOptions = b => {
                    this.setValues(b)
                };
                _.Ji("search_impl")
            }
            changed() {
                const a = this;
                var b = this.get("map");
                let c = null;
                b && (c = b.__gm, b = Number(c.get("blockingLayerCount")) || 0, c.set("blockingLayerCount", b + 1), c.set("disableLabelingHysteresis", this.get("disableLabelingHysteresis")), c.set("tilePrefetchEnabled", this.get("tilePrefetchEnabled")));
                _.Ji("search_impl").then(d => {
                    d.Eg(a);
                    c && (d = Number(c.get("blockingLayerCount")) || 0, c.set("blockingLayerCount",
                        d - 1))
                })
            }
            static enableFeatureMapEventsRasterOnly(a) {
                if (_.Km[15]) {
                    var b = a.__gm.Wg;
                    if (!Kya.has(a)) {
                        Kya.add(a);
                        var c = [],
                            d = (f, g) => {
                                f = Fya(f);
                                f.length && _.hk(a, g, f)
                            },
                            e = () => {
                                for (; c.length > 0;) c.pop().remove();
                                b.forEach(f => {
                                    if (f = f.data) c.push(_.Vj(f, "insert", g => {
                                        d(g, "addfeatures")
                                    })), c.push(_.Vj(f, "remove", g => {
                                        d(g, "removefeatures")
                                    }))
                                })
                            };
                        b.addListener("insert_at", e);
                        b.addListener("remove_at", e);
                        b.addListener("set_at", e);
                        e()
                    }(() => {
                        const f = [];
                        b.forEach(g => {
                            f.push(...Gya(g))
                        });
                        f.length && _.hk(a, "addfeatures", f)
                    })()
                }
            }
        };
    FD.enableFeatureMapEventsRasterOnly = FD.enableFeatureMapEventsRasterOnly;
    _.Ok(FD.prototype, {
        map: _.Kp
    });
    _.ka.google.maps.search = {
        GoogleLayer: FD
    };
    _.Ki("search", {});
});