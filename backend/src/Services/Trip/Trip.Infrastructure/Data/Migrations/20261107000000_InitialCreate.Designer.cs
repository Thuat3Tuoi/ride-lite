using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Trip.Domain.Enumerators;
using Trip.Infrastructure.Data.DataContext;

#nullable disable

namespace Trip.Infrastructure.Data.Migrations
{
    [DbContext(typeof(TripDbContext))]
    [Migration("20261107000000_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Trip.Domain.Entities.TripEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("AcceptedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("accepted_at");

                    b.Property<string>("CancelReason")
                        .HasColumnType("text")
                        .HasColumnName("cancel_reason");

                    b.Property<DateTime?>("CancelledAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("cancelled_at");

                    b.Property<CancelledBy?>("CancelledBy")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)")
                        .HasColumnName("cancelled_by");

                    b.Property<decimal?>("Cost")
                        .HasPrecision(14, 0)
                        .HasColumnType("numeric(14,0)")
                        .HasColumnName("cost");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("completed_at");

                    b.Property<long?>("DriverId")
                        .HasColumnType("bigint")
                        .HasColumnName("driver_id");

                    b.Property<DateTime?>("DriverArrivedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("driver_arrived_at");

                    b.Property<int>("DriverRejectCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("driver_reject_count");

                    b.Property<decimal>("DropoffLatitude")
                        .HasPrecision(10, 7)
                        .HasColumnType("numeric(10,7)")
                        .HasColumnName("dropoff_latitude");

                    b.Property<decimal>("DropoffLongitude")
                        .HasPrecision(10, 7)
                        .HasColumnType("numeric(10,7)")
                        .HasColumnName("dropoff_longitude");

                    b.Property<string>("DropoffAddress")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("dropoff_address");

                    b.Property<string>("PickupAddress")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("pickup_address");

                    b.Property<decimal>("PickupLatitude")
                        .HasPrecision(10, 7)
                        .HasColumnType("numeric(10,7)")
                        .HasColumnName("pickup_latitude");

                    b.Property<decimal>("PickupLongitude")
                        .HasPrecision(10, 7)
                        .HasColumnType("numeric(10,7)")
                        .HasColumnName("pickup_longitude");

                    b.Property<long>("RiderId")
                        .HasColumnType("bigint")
                        .HasColumnName("rider_id");

                    b.Property<DateTime?>("StartedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("started_at");

                    b.Property<TripStatus>("Status")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("status")
                        .HasConversion<string>();

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.ToTable("trips", null, t =>
                        {
                            t.HasCheckConstraint("ck_trips_cancelled_by", "cancelled_by IS NULL OR cancelled_by IN ('Rider', 'Driver', 'System')");
                            t.HasCheckConstraint("ck_trips_status", "status IN ('Requested', 'Quoted', 'AwaitingDriver', 'EnRoute', 'DriverArrived', 'PickedUp', 'DroppedOff', 'PaymentPending', 'Paid', 'Cancelled', 'Failed')");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
