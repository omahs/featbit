import os
from typing import Any, Callable, Optional
from uuid import UUID

import numpy as np
from flask import jsonify


def str_to_bool(value: Any) -> bool:
    if not value:
        return False
    return str(value).lower() in ("y", "yes", "t", "true", "on", "1")


def is_valid_uuid(uuid_to_test, version=4):

    try:
        uuid_obj = UUID(uuid_to_test, version=version)
    except ValueError:
        return False
    return str(uuid_obj) == uuid_to_test


def get_from_env(key: str, default: Any = None, *, type_cast: Optional[Callable[[Any], Any]] = None) -> Any:
    value = os.getenv(key)
    if value is None or value == "":
        return default
    if type_cast is not None:
        return type_cast(value)
    return value


def internal_error_handler(e: Exception):
    return jsonify(code=500, error=str(e), data={}), 500


class SingletonDecorator:
    def __init__(self, klass):
        self.klass = klass
        self.instance = None

    def __call__(self, *args, **kwds):
        if self.instance is None:
            self.instance = self.klass(*args, **kwds)
        return self.instance


def format_float_positional(value: float) -> Optional[str]:
    return None if value is None else np.format_float_positional(value, precision=10, trim='-')