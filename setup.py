"""
TestRepo - Python Utility Library
Setup configuration for package installation.
"""

from setuptools import setup, find_packages

with open("README.md", "r", encoding="utf-8") as fh:
    long_description = fh.read()

setup(
    name="testrepo",
    version="1.0.0",
    author="TestRepo Contributors",
    author_email="contributors@example.com",
    description="A comprehensive Python utility library for data validation, string manipulation, collections, and HTTP requests",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://github.com/example/testrepo",
    packages=find_packages(),
    classifiers=[
        "Development Status :: 4 - Beta",
        "Intended Audience :: Developers",
        "License :: OSI Approved :: MIT License",
        "Operating System :: OS Independent",
        "Programming Language :: Python :: 3",
        "Programming Language :: Python :: 3.8",
        "Programming Language :: Python :: 3.9",
        "Programming Language :: Python :: 3.10",
        "Programming Language :: Python :: 3.11",
        "Programming Language :: Python :: 3.12",
        "Topic :: Software Development :: Libraries :: Python Modules",
        "Topic :: Utilities",
        "Typing :: Typed",
    ],
    python_requires=">=3.8",
    install_requires=[],  # No external dependencies
    extras_require={
        "dev": [
            "pytest>=7.0.0",
            "pytest-cov>=4.0.0",
            "black>=23.0.0",
            "mypy>=1.0.0",
            "flake8>=6.0.0",
        ],
    },
    keywords="utilities, validation, strings, collections, http, api",
    project_urls={
        "Bug Reports": "https://github.com/example/testrepo/issues",
        "Source": "https://github.com/example/testrepo",
        "Documentation": "https://github.com/example/testrepo#readme",
    },
)
