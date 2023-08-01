#!/bin/bash

set -eu

VERSION="v9.6.0"
REPO_URL="https://github.com/libvirt/libvirt.git"

SHDIR=`cd $(dirname $0); pwd`

WORKDIR=`mktemp -d`
trap 'popd; rm -rf ${WORKDIR}; sudo rm -rf /tmp/libvirt' EXIT

# Build libvirt.
pushd ${WORKDIR}
git clone --depth 1 "${REPO_URL}" -b "${VERSION}" && cd libvirt

meson build \
    -Dexpensive_tests=disabled \
    -Ddocs=disabled \
    -Dtests=disabled \
    -Ddriver_bhyve=disabled \
    -Ddriver_esx=disabled \
    -Ddriver_hyperv=disabled \
    -Ddriver_interface=disabled \
    -Ddriver_libvirtd=disabled \
    -Ddriver_libxl=disabled \
    -Ddriver_lxc=disabled \
    -Ddriver_ch=disabled \
    -Ddriver_network=disabled \
    -Ddriver_openvz=disabled \
    -Ddriver_qemu=disabled \
    -Ddriver_secrets=disabled \
    -Ddriver_test=disabled \
    -Ddriver_vbox=disabled \
    -Ddriver_vmware=disabled \
    -Ddriver_vz=disabled \
    -Dsecdriver_apparmor=disabled \
    -Dapparmor_profiles=disabled \
    -Dsecdriver_selinux=disabled \
    -Dstorage_dir=disabled \
    -Dstorage_disk=disabled \
    -Dstorage_fs=disabled \
    -Dstorage_gluster=disabled \
    -Dstorage_iscsi=disabled \
    -Dstorage_iscsi_direct=disabled \
    -Dstorage_lvm=disabled \
    -Dstorage_mpath=disabled \
    -Dstorage_rbd=disabled \
    -Dstorage_scsi=disabled \
    -Dstorage_vstorage=disabled \
    -Dstorage_zfs=disabled \
    -Ddtrace=disabled \
    -Dfirewalld=disabled \
    -Dfirewalld_zone=disabled \
    -Dhost_validate=disabled \
    -Dlogin_shell=disabled \
    -Dnss=disabled \
    -Dnumad=disabled \
    -Dpm_utils=disabled \
    -Dsysctl_config=disabled \
    -Dprefix=/tmp/libvirt

ninja -C build
sudo ninja -C build install

# Generate header.
dotnet run --project "${SHDIR}/Gen/Gen.csproj" -- \
    '/tmp/libvirt/include/libvirt/libvirt.h' \
    -I '/tmp/libvirt/include' \
    -I '/usr/lib/gcc/x86_64-linux-gnu/11/include'
mv -f Generated.cs "${SHDIR}/../LibvirtHeader/"
