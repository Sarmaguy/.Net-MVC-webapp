using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RPPP_WebApp.Models
{
    public partial class RPPP09Context : DbContext
    {
        public RPPP09Context()
        {
        }

        public RPPP09Context(DbContextOptions<RPPP09Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Autocestum> Autocesta { get; set; }
        public virtual DbSet<CestovniObjekt> CestovniObjekts { get; set; }
        public virtual DbSet<Cjenik> Cjeniks { get; set; }
        public virtual DbSet<Dionica> Dionicas { get; set; }
        public virtual DbSet<Dogadaj> Dogadajs { get; set; }
        public virtual DbSet<Enc> Encs { get; set; }
        public virtual DbSet<Kamera> Kameras { get; set; }
        public virtual DbSet<KontrolaProlaska> KontrolaProlaskas { get; set; }
        public virtual DbSet<MultimedijaOdmoriste> MultimedijaOdmoristes { get; set; }
        public virtual DbSet<MultimedijaPrateciSadrzaj> MultimedijaPrateciSadrzajs { get; set; }
        public virtual DbSet<NaplatnaPostaja> NaplatnaPostajas { get; set; }
        public virtual DbSet<ObilazniPravac> ObilazniPravacs { get; set; }
        public virtual DbSet<Odmoriste> Odmoristes { get; set; }
        public virtual DbSet<PrateciSadrzaj> PrateciSadrzajs { get; set; }
        public virtual DbSet<Sezona> Sezonas { get; set; }
        public virtual DbSet<SkupinaVozila> SkupinaVozilas { get; set; }
        public virtual DbSet<TipCestovnogObjektum> TipCestovnogObjekta { get; set; }
        public virtual DbSet<VlasnikAutoceste> VlasnikAutocestes { get; set; }
        public virtual DbSet<VrstaDogadaja> VrstaDogadajas { get; set; }
        public virtual DbSet<VrstaKamere> VrstaKameres { get; set; }
        public virtual DbSet<VrstaSadrzaja> VrstaSadrzajas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Autocestum>(entity =>
            {
                entity.HasKey(e => e.AutocestaId);

                entity.Property(e => e.AutocestaIme)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Oibvlasnika)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("OIBVlasnika");

                entity.HasOne(d => d.OibvlasnikaNavigation)
                    .WithMany(p => p.Autocesta)
                    .HasForeignKey(d => d.Oibvlasnika)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Autocesta_VlasnikAutoceste");
            });

            modelBuilder.Entity<CestovniObjekt>(entity =>
            {
                entity.HasKey(e => e.Coid);

                entity.ToTable("CestovniObjekt");

                entity.Property(e => e.Coid)
                    .ValueGeneratedNever()
                    .HasColumnName("COId");

                entity.Property(e => e.Conaziv)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CONaziv");

                entity.Property(e => e.Costacionaza)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("COStacionaza");

                entity.HasOne(d => d.Dionica)
                    .WithMany(p => p.CestovniObjekts)
                    .HasForeignKey(d => d.DionicaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CestovniObjekt_Dionica");
            });

            modelBuilder.Entity<Cjenik>(entity =>
            {
                entity.HasKey(e => e.CjenikRelacija);

                entity.ToTable("Cjenik");

                entity.Property(e => e.CjenikRelacija)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CijenaEur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CijenaEUR");

                entity.Property(e => e.CijenaHrk)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CijenaHRK");

                entity.Property(e => e.NpId).HasColumnName("npId");

                entity.HasOne(d => d.Np)
                    .WithMany(p => p.Cjeniks)
                    .HasForeignKey(d => d.NpId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cjenik_NaplatnaPostaja");

                entity.HasOne(d => d.Sezona)
                    .WithMany(p => p.Cjeniks)
                    .HasForeignKey(d => d.SezonaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cjenik_Sezona");

                entity.HasOne(d => d.SkupinaVozila)
                    .WithMany(p => p.Cjeniks)
                    .HasForeignKey(d => d.SkupinaVozilaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cjenik_SkupinaVozila");
            });

            modelBuilder.Entity<Dionica>(entity =>
            {
                entity.ToTable("Dionica");

                entity.HasOne(d => d.IdAutocesteNavigation)
                    .WithMany(p => p.Dionicas)
                    .HasForeignKey(d => d.IdAutoceste)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dionica_Autocesta");
            });

            modelBuilder.Entity<Dogadaj>(entity =>
            {
                entity.ToTable("Dogadaj");

                entity.Property(e => e.DogadajDatumVrijeme).HasColumnType("date");

                entity.Property(e => e.DogadajOpis)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdDioniceNavigation)
                    .WithMany(p => p.Dogadajs)
                    .HasForeignKey(d => d.IdDionice)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dogadaj_Dionica");

                entity.HasOne(d => d.IdVrsteDogadajaNavigation)
                    .WithMany(p => p.Dogadajs)
                    .HasForeignKey(d => d.IdVrsteDogadaja)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Dogadaj_VrstaDogadaja");
            });

            modelBuilder.Entity<Enc>(entity =>
            {
                entity.ToTable("ENC");

                entity.Property(e => e.EncId).ValueGeneratedNever();

                entity.HasOne(d => d.SkupinaVozila)
                    .WithMany(p => p.Encs)
                    .HasForeignKey(d => d.SkupinaVozilaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ENC_SkupinaVozila");
            });

            modelBuilder.Entity<Kamera>(entity =>
            {
                entity.ToTable("Kamera");


                entity.Property(e => e.KameraKoordinate)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.KameraSmjer)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.KameraUrl)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("KameraURL");

                entity.HasOne(d => d.Autocesta)
                    .WithMany(p => p.Kameras)
                    .HasForeignKey(d => d.AutocestaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Kamera_Autocesta");

                entity.HasOne(d => d.VrstaKamere)
                    .WithMany(p => p.Kameras)
                    .HasForeignKey(d => d.VrstaKamereId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Kamera_VrstaKamere");
            });

            modelBuilder.Entity<KontrolaProlaska>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("KontrolaProlaska");

                entity.Property(e => e.EndingNpid).HasColumnName("EndingNPId");

                entity.Property(e => e.StartingNpid).HasColumnName("StartingNPId");

                entity.HasOne(d => d.Enc)
                    .WithMany()
                    .HasForeignKey(d => d.EncId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KontrolaProlaska_ENC");

                entity.HasOne(d => d.EndingNp)
                    .WithMany()
                    .HasForeignKey(d => d.EndingNpid)
                    .HasConstraintName("FK_KontrolaProlaska_NaplatnaPostaja1");

                entity.HasOne(d => d.StartingNp)
                    .WithMany()
                    .HasForeignKey(d => d.StartingNpid)
                    .HasConstraintName("FK_KontrolaProlaska_NaplatnaPostaja");
            });

            modelBuilder.Entity<MultimedijaOdmoriste>(entity =>
            {
                entity.ToTable("MultimedijaOdmoriste");

                entity.Property(e => e.MultimedijaOdmoristeUrl)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MultimedijaOdmoristeURL");

                entity.HasOne(d => d.Odmoriste)
                    .WithMany(p => p.MultimedijaOdmoristes)
                    .HasForeignKey(d => d.OdmoristeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MultimedijaOdmoriste_Odmoriste");
            });

            modelBuilder.Entity<MultimedijaPrateciSadrzaj>(entity =>
            {
                entity.ToTable("MultimedijaPrateciSadrzaj");

                entity.Property(e => e.MultimedijaPrateciSadrzajUrl)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MultimedijaPrateciSadrzajURL");

                entity.HasOne(d => d.PrateciSadrzaj)
                    .WithMany(p => p.MultimedijaPrateciSadrzajs)
                    .HasForeignKey(d => d.PrateciSadrzajId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MultimedijaPrateciSadrzaj_PrateciSadrzaj");
            });

            modelBuilder.Entity<NaplatnaPostaja>(entity =>
            {
                entity.HasKey(e => e.NpId);

                entity.ToTable("NaplatnaPostaja");

                entity.Property(e => e.NpId)
                    .ValueGeneratedNever()
                    .HasColumnName("npId");

                entity.Property(e => e.NpIme)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("npIme");

                entity.Property(e => e.NpKoordinate)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("npKoordinate");

                entity.HasOne(d => d.Autocesta)
                    .WithMany(p => p.NaplatnaPostajas)
                    .HasForeignKey(d => d.AutocestaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NaplatnaPostaja_Autocesta");
            });

            modelBuilder.Entity<ObilazniPravac>(entity =>
            {
                entity.ToTable("ObilazniPravac");

                entity.Property(e => e.ObilazniPravacNaziv)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdAutocesteNavigation)
                    .WithMany(p => p.ObilazniPravacs)
                    .HasForeignKey(d => d.IdAutoceste)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ObilazniPravac_Autocesta");
            });

            modelBuilder.Entity<Odmoriste>(entity =>
            {
                entity.ToTable("Odmoriste");

                entity.Property(e => e.OdmoristeKoordinate)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OdmoristeNaziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Autocesta)
                    .WithMany(p => p.Odmoristes)
                    .HasForeignKey(d => d.AutocestaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Odmoriste_Autocesta");
            });

            modelBuilder.Entity<PrateciSadrzaj>(entity =>
            {
                entity.ToTable("PrateciSadrzaj");

                entity.Property(e => e.PrateciSadrzajKoordinate)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrateciSadrzajNaziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrateciSadrzajRadnoVrijeme)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Odmoriste)
                    .WithMany(p => p.PrateciSadrzajs)
                    .HasForeignKey(d => d.OdmoristeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrateciSadrzaj_Odmoriste");

                entity.HasOne(d => d.VrstaSadrzaja)
                    .WithMany(p => p.PrateciSadrzajs)
                    .HasForeignKey(d => d.VrstaSadrzajaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrateciSadrzaj_VrstaSadrzaja");
            });

            modelBuilder.Entity<Sezona>(entity =>
            {
                entity.ToTable("Sezona");

                entity.Property(e => e.SezonaId).ValueGeneratedNever();

                entity.Property(e => e.NazivSezone)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SkupinaVozila>(entity =>
            {
                entity.ToTable("SkupinaVozila");

                entity.Property(e => e.SkupinaVozilaId).ValueGeneratedNever();

                entity.Property(e => e.NazivSkupineVozila)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipCestovnogObjektum>(entity =>
            {
                entity.HasKey(e => e.CotipId);

                entity.Property(e => e.CotipId)
                    .ValueGeneratedNever()
                    .HasColumnName("COTipId");

                entity.Property(e => e.Coid).HasColumnName("COId");

                entity.Property(e => e.CotipIme)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("COTipIme");

                entity.HasOne(d => d.Co)
                    .WithMany(p => p.TipCestovnogObjekta)
                    .HasForeignKey(d => d.Coid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TipCestovnogObjekta_CestovniObjekt");
            });

            modelBuilder.Entity<VlasnikAutoceste>(entity =>
            {
                entity.HasKey(e => e.Oib);

                entity.ToTable("VlasnikAutoceste");

                entity.Property(e => e.Oib)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("OIB");

                entity.Property(e => e.VlasnikIme)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VrstaDogadaja>(entity =>
            {
                entity.ToTable("VrstaDogadaja");

                entity.Property(e => e.VrstaDogadajaNaziv)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsFixedLength();
            });

            modelBuilder.Entity<VrstaKamere>(entity =>
            {
                entity.ToTable("VrstaKamere");

                entity.Property(e => e.VrstaKamereNaziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VrstaSadrzaja>(entity =>
            {
                entity.ToTable("VrstaSadrzaja");

                entity.Property(e => e.VrstaSadrzajaNaziv)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
