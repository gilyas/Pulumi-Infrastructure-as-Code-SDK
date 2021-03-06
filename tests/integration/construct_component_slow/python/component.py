# Copyright 2016-2021, Pulumi Corporation.  All rights reserved.

from typing import Any, Optional

import pulumi

class Component(pulumi.ComponentResource):
    def __init__(self, name: str, opts: Optional[pulumi.ResourceOptions] = None):
        props = dict()
        super().__init__("testcomponent:index:Component", name, props, opts, True)
